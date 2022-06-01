using UnityEngine;

public class HandSoapController : MonoBehaviour, IInventoryItemInstance
{
   public SoapModel Soap { get; private set; }

   [SerializeField] private GameObject _soapPrefab = null;

   private InventoryComponent _inventoryComponent;
   private SpriteRenderer _spriteRenderer;
   private GameObject _playerGameObject;

   public float GetHeight()
   {
      return _spriteRenderer?.bounds.size.y ?? 0;
   }

   public void Drop()
   {
      var instance = Instantiate(_soapPrefab, _playerGameObject?.transform.position ?? transform.position, Quaternion.identity, null);
      var soapCollectable = instance.GetComponentInChildren<CollectableSoapController>();
      soapCollectable?.InitializeData(Soap);
      soapCollectable.TriggerCollectableCooldown();

      SoapSpawnManager.Instance.AllSoapTransforms.Add(soapCollectable.transform);
      _inventoryComponent?.RemoveItem(this);
   }

   public void InitializeData(SoapModel soap)
   {
      Soap = soap;
   }

   public IInventoryItem ToInventoryItem()
   {
      return Soap;
   }

   private void Awake()
   {
      _playerGameObject = GameObject.FindGameObjectWithTag("Player");
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _inventoryComponent = _playerGameObject?.GetComponent<InventoryComponent>();
   }
}