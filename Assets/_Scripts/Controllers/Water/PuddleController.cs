using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(SpriteRenderer)), ExecuteInEditMode]
public class PuddleController : MonoBehaviour
{
   public float Volume
   {
      get
      {
         return _volume;
      }
      set
      {
         _volume = (value > 0f) ? value : 0f;
         _volumeChanged = true;
      }
   }

   public bool IsDirty { get { return Hygiene < 0f; } }

   public float Hygiene;

   [SerializeField] private Sprite[] _smallDroplets = null;
   [SerializeField] private Sprite[] _mediumDroplets = null;
   [SerializeField] private Sprite[] _largeDroplets = null;
   [SerializeField] private LayerMask _splashLayers = (LayerMask)0;
   [SerializeField] private Color _cleanColor = Color.blue;
   [SerializeField] private Color _dirtyColor = Color.green;

   private SpriteRenderer _spriteRenderer;
   private PolygonCollider2D _collider;

   [SerializeField] private float _volume;
   [SerializeField, HideInInspector] private bool _volumeChanged;

   private bool _flaggedForDestroy;
   private float _mediumPuddleThreshold = 3f;
   private float _largePuddleThreshold = 6f;

   public void Initialize(float volume, float hygiene = Constants.HYGIENE_MAX)
   {
      Volume = volume;
      Hygiene = hygiene;
   }

   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _collider = GetComponent<PolygonCollider2D>();

      _spriteRenderer.sortingOrder = Constants.SORTING_ORDER_PUDDLE;
   }

   private void Update()
   {
      if (_volumeChanged && !_flaggedForDestroy)
      {
         //figure out which sprite to change to
         if (Volume > _largePuddleThreshold)
         {
            //since its the largest sprite, only change it if its not already a large one
            if (_largeDroplets != null && _largeDroplets.Length > 0 && !_largeDroplets.Contains(_spriteRenderer.sprite))
            {
               _spriteRenderer.sprite = _largeDroplets[UnityEngine.Random.Range(0, _largeDroplets.Length)];

               transform.localScale = Vector3.zero;
            }
         }
         else if (Volume > _mediumPuddleThreshold)
         {
            if (_mediumDroplets != null && _mediumDroplets.Length > 0 && !_mediumDroplets.Contains(_spriteRenderer.sprite))
            {
               _spriteRenderer.sprite = _mediumDroplets[UnityEngine.Random.Range(0, _mediumDroplets.Length)];

               transform.localScale = Vector3.zero;
            }
         }
         else
         {
            if (_smallDroplets != null && _smallDroplets.Length > 0 && !_smallDroplets.Contains(_spriteRenderer.sprite))
            {
               _spriteRenderer.sprite = _smallDroplets[UnityEngine.Random.Range(0, _smallDroplets.Length)];

               transform.localScale = Vector3.zero;
            }
         }

         //create collider based on chosen sprite
         if (_spriteRenderer.sprite != null)
         {
            var points = new List<Vector2>();
            _spriteRenderer.sprite.GetPhysicsShape(0, points);
            _collider.SetPath(0, points.ToArray());
         }

         //get volume in nearby area of puddles and over a certain volume, tell the largest puddle in the set to assimilate the others
         var hits = Physics2D.OverlapCircleAll(transform.position, _collider.bounds.extents.y, _splashLayers);
         if (hits.Length > 1)
         {
            var hitPuddles = new List<PuddleController>();
            foreach (var hit in hits)
            {
               var hitPuddle = hit.GetComponent<PuddleController>();
               if (hitPuddle != null && !hitPuddle._flaggedForDestroy) hitPuddles.Add(hitPuddle);
            }

            var volumeSum = hitPuddles.Sum(p => p.Volume);
            if (volumeSum > _largePuddleThreshold)
            {
               var largestHitPuddle = hitPuddles.OrderByDescending(p => p.Volume).FirstOrDefault();
               largestHitPuddle.Assimilation(hitPuddles);
            }
         }

         _volumeChanged = false;

         if (Volume < 0f) DeleteSafely();
      }

      if (!_flaggedForDestroy)
      {
         //scale the sprite based on volume, to bridge size gap between sprite types
         transform.localScale = Vector3.Lerp(transform.localScale, CalculateScale(transform.localScale), Time.deltaTime * 10f);

         //updating dirty or clean state
         _spriteRenderer.color = Hygiene < 0f ? _dirtyColor : _cleanColor;
      }
   }


   private void OnDisable()
   {
      StopAllCoroutines();
   }

   private void OnDrawGizmos()
   {
#if UNITY_EDITOR
      
      if (Volume >= 3f)
      {
         UnityEditor.Handles.Label(transform.position + new Vector3(0, 0.33f), Volume.ToString("#.00"), UnityEditor.EditorStyles.boldLabel);
      }
#endif
   }

   private Vector3 CalculateScale(Vector3 scale)
   {
      //this scaling will entirely depend on size and shape of final water sprites
      if (Volume < _mediumPuddleThreshold)
      {
         //small
         scale = Vector3.one * (Volume / _mediumPuddleThreshold);
      }
      else if (Volume < _largePuddleThreshold)
      {
         //medium
         scale = Vector3.one * (Volume / _mediumPuddleThreshold);
      }
      else
      {
         //large
         scale = Vector3.one * (Mathf.Min(Volume, 12f) / (_mediumPuddleThreshold + _largePuddleThreshold));
      }

      return scale;
   }

   private void Assimilation(List<PuddleController> assimilatees)
   {
      foreach(var assimilatee in assimilatees)
      {
         if (assimilatee != this && !assimilatee._flaggedForDestroy)
         {
            assimilatee.Assimilate(this);
         }
      }
   }

   private void Assimilate(PuddleController assimilator)
   {
      assimilator.Volume += Volume;

      //proportionally merge hygiene
      assimilator.Hygiene += Mathf.Clamp((Volume / assimilator.Volume) * Hygiene, Constants.HYGIENE_MIN, Constants.HYGIENE_MAX);

      StartCoroutine(AssimilateCoroutine(assimilator.transform.position));
   }

   private IEnumerator AssimilateCoroutine(Vector3 target)
   {
      _flaggedForDestroy = true;

      //forced timeout just for low framerate or if something breaks
      float timeout = 5f;
      float timer = 0f;

      var distanceStepRate = 4f;
      var originalDistance = Vector3.Distance(transform.position, target);
      var originalScale = transform.localScale;

      while (Vector3.Distance(transform.position, target) > 0.01f || timer > timeout)
      {
         var iterationStartPosition = transform.position;

         transform.position = Vector3.MoveTowards(transform.position, target, distanceStepRate * Time.deltaTime);

         var proportionTravelled = Vector3.Distance(transform.position, iterationStartPosition) / originalDistance;
         transform.localScale += originalScale * proportionTravelled;

         timer += Time.deltaTime;

         yield return new WaitForEndOfFrame();
      }

      DestroyImmediate(gameObject);

      yield return null;
   }

   private void DeleteSafely()
   {
      if (!_flaggedForDestroy)
      {
         _flaggedForDestroy = true;
         Destroy(gameObject);
      }
   }

}
