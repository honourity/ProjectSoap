using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSoapAnchorController : MonoBehaviour
{
   [SerializeField] private Sprite _soapSprite = null;

   private List<GameObject> _soapInstances = new List<GameObject>();

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapAdded);
      Messaging.AddListener(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapRemoved);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapAdded);
      Messaging.RemoveListener(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapRemoved);
   }

   private void OnInventorySoapAdded()
   {
      if (_soapSprite != null)
      {
         var instance = new GameObject();
         instance.transform.SetParent(transform);
         instance.transform.position = transform.position + (Vector3.up * (_soapInstances.Count * _soapSprite.bounds.size.y));
         
         //actually, parent player gameobject probably has a spritelayercontroller which handles all children already
         //var layerController = instance.AddComponent<SpriteLayerController>();
         //layerController.Dynamic = true;
         var renderer = instance.AddComponent<SpriteRenderer>();
         renderer.sprite = _soapSprite;
         _soapInstances.Add(instance);
      }
   }

   private void OnInventorySoapRemoved()
   {
      if (_soapInstances.Count > 0)
      {
         var soapToRemove = _soapInstances.Last();
         Destroy(soapToRemove);
         _soapInstances.Remove(soapToRemove);
      }
   }

}
