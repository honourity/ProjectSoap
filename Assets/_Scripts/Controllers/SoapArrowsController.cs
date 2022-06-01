using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class SoapArrowsController : MonoBehaviour
{
   private struct ArrowCoroutinePair
   {
      public GameObject Arrow;
      public Coroutine Coroutine;
   }

   [SerializeField] private GameObject _arrowPrefab;
   [SerializeField] private Vector3 _arrowPositionOffset;

   private Dictionary<Transform, ArrowCoroutinePair> _soapsArrowsCoroutines;

   private ObjectPool<GameObject> _arrowsPool;
   private Transform _player { get { return GameManager.Instance.Player.transform; } }

   private int counter = 0;

   private void Start()
   {
      _soapsArrowsCoroutines = new Dictionary<Transform, ArrowCoroutinePair>();
      _arrowsPool = new ObjectPool<GameObject>(CreateArrow, 1);
   }

   private GameObject CreateArrow()
   {
      GameObject newArrow = Instantiate(_arrowPrefab);
      newArrow.name = newArrow.name += " " + counter++;
      newArrow.SetActive(false);
      return newArrow;
   }

   public void Update()
   {
      foreach (Transform soap in SoapSpawnManager.Instance.AllSoapTransforms)
      {
         //If a new soap was created
         if (!_soapsArrowsCoroutines.ContainsKey(soap))
         {
            ArrowCoroutinePair newArrowCoroutinePair;
            newArrowCoroutinePair.Arrow = _arrowsPool.GetItem();
            newArrowCoroutinePair.Coroutine = StartCoroutine(DoArrows(soap, newArrowCoroutinePair.Arrow));
            _soapsArrowsCoroutines.Add(soap, newArrowCoroutinePair);
         }

      }
   }

   private IEnumerator DoArrows(Transform soap, GameObject arrow)
   {
      Vector3 direction = Vector3.positiveInfinity;
      arrow.SetActive(true);
      bool breakage = false;
      CollectableSoapController soapController = soap.GetComponent<CollectableSoapController>();
      SpriteRenderer spriteRenderer = arrow.GetComponent<SpriteRenderer>();

      Coroutine blinkingCoroutine = StartCoroutine(BlinkArrows(spriteRenderer, soapController.DestroyTimer));

      while (!breakage)
      {
         //Check if the soap still exists, stop if it doesn't
         if (soap == null)
         {
            arrow.SetActive(false);
            _arrowsPool.Release(arrow);
            StopCoroutine(blinkingCoroutine);
            spriteRenderer.enabled = true;
            breakage = true;
            yield break;
         }
         else
         {
            direction = soap.position - _player.position;
            Debug.DrawRay(_player.position, direction, Color.red);
            Vector2 intersection = Vector2.zero;

            Vector3 topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight));
            Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
            Vector3 bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0));
            Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);

            topLeft.z = 0; topRight.z = 0; bottomRight.z = 0; bottomLeft.z = 0;

            //CHECK TOP
            bool foundIntersection = Utilities.LineIntersection(_player.position, soap.position, topLeft, topRight, ref intersection);

            if (!foundIntersection)
            {
               //CHECK RIGHT
               foundIntersection = Utilities.LineIntersection(_player.position, soap.position, topRight, bottomRight, ref intersection);

               if (!foundIntersection)
               {
                  //CHECK BOTTOM
                  foundIntersection = Utilities.LineIntersection(_player.position, soap.position, bottomRight, bottomLeft, ref intersection);

                  if (!foundIntersection)
                  {
                     //CHECK RIGHT
                     foundIntersection = Utilities.LineIntersection(_player.position, soap.position, bottomLeft, topLeft, ref intersection);

                     if (!foundIntersection)
                     {
                        //If we reach this point, then the soap must be in the camera frame
                        arrow.transform.up = Vector2.down;
                        arrow.transform.position = soap.position + _arrowPositionOffset;
                        yield return null;
                        continue;
                     }
                  }
               }
            }

            //Reaching this point means the soap is outside the camera 
            arrow.transform.up = direction;
            arrow.transform.position = intersection;

            yield return null;
         }
      }
   }

   private IEnumerator BlinkArrows(SpriteRenderer arrowSprite, float maxTime)
   {
      float halfTime = maxTime / 2f;
      float timeLeft = halfTime;

      yield return new WaitForSeconds(halfTime);

      Run run = null;

      run = Run.Every(0f, 1f, () =>
      {
         timeLeft--;
         if (timeLeft < 0)
         {
            run.Abort();
         }
      });

      while (timeLeft >= 0)
      {
         yield return new WaitForSeconds(Mathf.Max(timeLeft, 1f) / 8f);
         arrowSprite.enabled = false;
         yield return new WaitForSeconds(Mathf.Max(timeLeft, 1f) / 8f);
         arrowSprite.enabled = true;
      }

      yield return null;
   }


}
