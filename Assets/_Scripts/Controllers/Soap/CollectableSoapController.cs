using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectableSoapController : MonoBehaviour, IInventoryItemInstance
{
   public float Hygiene
   {
      get
      {
         return _hygiene;
      }

      set
      {
         _hygiene = value;
         if (_hygiene <= 0f)
         {
            AudioManager.Instance.Play(_consumedSound);

            Destroy(gameObject);
         }
      }
   }

   public bool IsCollectable = true;
   public bool TimedDestroy = true;
   public float DestroyTimer = 10f;
   [SerializeField] private Enums.Size _size = Enums.Size.MEDIUM;
   [SerializeField] private float _hygiene = 100f;
   [SerializeField] private LayerMask _throwCollisionIgnoreLayers = 0;

   [Header("Sounds")]
   [SerializeField] private AudioClip _throwSound = null;
   [SerializeField] private AudioClip _bounceSound = null;
   [SerializeField] private AudioClip _collectSound = null;
   [SerializeField] private AudioClip _consumedSound = null;

   private Guid _id = Guid.NewGuid();
   private bool _flaggedForDestroy;

   public bool AttemptCollect()
   {
      if (!_flaggedForDestroy)
      {
         Destroy(gameObject);
         _flaggedForDestroy = true;
         IsCollectable = false;

         AudioManager.Instance.Play(_collectSound);

         return true;
      }
      else
      {
         return false;
      }
   }

   public void TriggerCollectableCooldown()
   {
      StopCoroutine(IsCollectableCooldown());
      StartCoroutine(IsCollectableCooldown());
   }

   public IInventoryItem ToInventoryItem()
   {
      return new SoapModel(_id, _size, Hygiene);
   }

   public IEnumerator SoapTrajectoryCoroutine(Vector3 originOffset, Vector3 to)
   {
      var soapSpriteRenderer = GetComponent<SpriteRenderer>();
      var soapCollider = GetComponent<Collider2D>();

      //creating a 'ghost' soap which can pass through any object
      // temporarily turning the real soap into a shadow for interaction with the world
      var ghostSoap = new GameObject("SoapGhost");
      var childSpriteRenderer = ghostSoap.AddComponent<SpriteRenderer>();
      childSpriteRenderer.sprite = soapSpriteRenderer.sprite;
      var originalColor = soapSpriteRenderer.color;
      childSpriteRenderer.color = originalColor;
      soapSpriteRenderer.color = Color.black;
      var originalScale = transform.localScale;

      transform.localPosition += new Vector3(originOffset.x, 0);

      IsCollectable = false;

      var timeout = 2f;
      var moveSpeed = 30f;
      var originalDistance = Vector3.Distance(transform.position, to);
      var distanceRemaining = originalDistance;
      var yOffsetGhostSoap = 0f;

      AudioManager.Instance.Play(_throwSound, 0.5f);

      while (!_flaggedForDestroy && enabled && distanceRemaining > 0.5f && timeout > 0f)
      {
         //soap ghost: skip the dynamic sprite ordering, and use 1 more than the shadow
         childSpriteRenderer.sortingOrder = soapSpriteRenderer.sortingOrder + 1;

         //cache some calculations and external references
         distanceRemaining = Vector3.Distance(transform.position, to);
         var deltaTime = Time.deltaTime;
         var iterationMoveDistance = moveSpeed * deltaTime;

         //handling hitting obstacles
         var hit = Physics2D.Raycast(transform.position, to - transform.position, iterationMoveDistance + soapCollider.bounds.extents.y, ~_throwCollisionIgnoreLayers);
         if (hit && !hit.collider.isTrigger)
         {
            //hit something so 'bounce' in oposite direction
            var reverseDirection = Vector3.Reflect(to - transform.position, hit.normal);

            AudioManager.Instance.Play(_bounceSound, 1f);

            //update 'to' target, and reduce remaining distance
            to = transform.position + (reverseDirection * 0.5f);
         }

         //shadow of soap takes direct path
         transform.position = Vector3.MoveTowards(transform.position, to, iterationMoveDistance);

         //soap takes ballistic trajectory
         if (distanceRemaining < (originalDistance / 2f))
         {
            yOffsetGhostSoap -= 20f * deltaTime;

            //soap has 'landed' on the ground
            if (originOffset.y + yOffsetGhostSoap < 0f)
            {
               moveSpeed = Mathf.Max(0.1f, moveSpeed - (moveSpeed * 10f * deltaTime));
               yOffsetGhostSoap = -originOffset.y;
               IsCollectable = true;
            }
         }
         ghostSoap.transform.position = transform.position + new Vector3(0, Mathf.Max(0f, originOffset.y + yOffsetGhostSoap), 0);

         //set soap shadow scale based on distance between shadow and ghost
         var yDifference = Vector3.Distance(ghostSoap.transform.position, transform.position);
         var shadowScale = yDifference.Map(0f, originOffset.y, 1f, 1.5f);
         var shadowAlpha = yDifference.Map(0f, originOffset.y, 0.9f, 0.3f);
         transform.localScale = Vector3.one * shadowScale;
         soapSpriteRenderer.color = new Color(soapSpriteRenderer.color.r, soapSpriteRenderer.color.g, soapSpriteRenderer.color.b, shadowAlpha);

         //emergency timeout just in case something screws up
         timeout -= deltaTime;

         yield return null;
      }


      //undo shadow effect and delete ghost soap
      if (!_flaggedForDestroy && enabled)
      {
         soapSpriteRenderer.color = originalColor;
         transform.localScale = originalScale;
         IsCollectable = true;
      }
      Destroy(ghostSoap);
   }


   public void InitializeData(SoapModel soap)
   {
      _id = soap.Id;
      _size = soap.Size;
      Hygiene = soap.Hygiene;
   }

   private void Start()
   {
      if (TimedDestroy)
      {
         Destroy(gameObject, DestroyTimer);
      }
   }

   private void OnDestroy()
   {
      StopAllCoroutines();

      _flaggedForDestroy = true;
      IsCollectable = false;

      Messaging.SendMessage(Enums.MessageType.SOAP_COLLECTABLE_DESTROYED, this);

      enabled = false;
   }

   private IEnumerator IsCollectableCooldown()
   {
      IsCollectable = false;

      yield return new WaitForSeconds(1f);

      IsCollectable = true;
   }
}