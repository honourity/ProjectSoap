using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapArrowsQuadrantsController : MonoBehaviour
{
   [SerializeField] private GameObject _northArrow = null;
   [SerializeField] private GameObject _eastArrow = null;
   [SerializeField] private GameObject _southArrow = null;
   [SerializeField] private GameObject _westArrow = null;

   private Transform _player;
   private Vector3 ScreenPoint;

   private void Awake()
   {
      _player = FindObjectOfType<PlayerController>().transform;
   }

   // Update is called once per frame
   void Update()
   {
      _northArrow.SetActive(false);
      _eastArrow.SetActive(false);
      _southArrow.SetActive(false);
      _westArrow.SetActive(false);

      foreach(Transform tr in SoapSpawnManager.Instance.AllSoapTransforms)
      {
         if (tr != null)
         {
            ScreenPoint = Camera.main.WorldToScreenPoint(tr.position);
            if (ScreenPoint.x < 0 || ScreenPoint.x > Camera.main.pixelWidth || ScreenPoint.y < 0 || ScreenPoint.y > Camera.main.pixelHeight)
            {
               if (Mathf.Abs(tr.position.x - _player.position.x) > Mathf.Abs(tr.position.y - _player.position.y))
               {
                  if (tr.position.x > _player.position.x)
                  {
                     _eastArrow.SetActive(true);
                  }
                  else
                  {
                     _westArrow.SetActive(true);
                  }
               }
               else
               {
                  if (tr.position.y > _player.position.y)
                  {
                     _northArrow.SetActive(true);
                  }
                  else
                  {
                     _southArrow.SetActive(true);
                  }
               }
            }
         }
      }
   }
}
