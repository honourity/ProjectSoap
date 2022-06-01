using System.Collections.Generic;
using UnityEngine;

public class SoapCountUIController : MonoBehaviour
{
   [SerializeField] private GameObject _uiSoapPrefab = null;
   [SerializeField] private List<GameObject> _allUISoap = new List<GameObject>();

   private InventoryComponent _inventoryComponent;

   private void Awake()
   {
      var playerGameObject = GameObject.FindGameObjectWithTag("Player");
      _inventoryComponent = playerGameObject.GetComponent<InventoryComponent>();
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapChanged);
      Messaging.AddListener(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapChanged);
   }

   private void OnDestroy()
   {
      Messaging.RemoveListener(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapChanged);
      Messaging.RemoveListener(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapChanged);
   }

   private void OnInventorySoapChanged()
   {
      var soaps = _inventoryComponent.GetItemsOfType<SoapModel>();

      //instantiate more ui soap if there isnt enough
      var difference = soaps?.Count - _allUISoap.Count;
      if (difference > 0)
      {
         for (var i = 0; i < difference; i++)
         {
            _allUISoap.Add(Instantiate(_uiSoapPrefab, transform));
         }
      }

      //disable all instances
      _allUISoap.ForEach(s => s.SetActive(false));

      //enable up till soapCount
      for(var i = 0; i < soaps?.Count; i++)
      {
         _allUISoap[i].SetActive(true);
      }
   }
}
