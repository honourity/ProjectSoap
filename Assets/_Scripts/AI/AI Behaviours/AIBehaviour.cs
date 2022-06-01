using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviour : MonoBehaviour
{
   public bool IgnorePlayer { get; set; }

#if UNITY_EDITOR
   [SerializeField] private bool _showComponents = false;
#endif

   [SerializeField, HideInInspector] protected List<AIModule> _allAIModules;

   protected AIModule _currentModule;
   protected MovementController _moveController;
   protected HygieneComponent _hygieneComponent;
   protected InventoryComponent _inventoryComponent;

   protected virtual void OnEnable() 
   {
      //Disable all other AIModules
      AIBehaviour[] allAIBehaviour = GetComponents<AIBehaviour>();

      for (int i = 0; i < allAIBehaviour.Length; i++)
      {
         if (allAIBehaviour[i] != this)
         {
            allAIBehaviour[i].enabled = false;
         }
      }
   }

   protected virtual void OnDisable()
   {
      for (int i = 0; i < _allAIModules?.Count; i++)
      {
         _allAIModules[i].enabled = false;
      }
   }

   protected virtual void Awake()
   {
      _moveController = GetComponent<MovementController>();
      _hygieneComponent = GetComponent<HygieneComponent>();
      _inventoryComponent = GetComponent<InventoryComponent>() ?? gameObject.AddComponent<InventoryComponent>();
   }

   protected virtual void Start()
   {
#if UNITY_EDITOR
      if (_showComponents && _allAIModules != null)
      {
         for (int i = 0; i < _allAIModules.Count; i++)
         {
            _allAIModules[i].hideFlags = HideFlags.None;
         }
      }
#endif
   }

   protected virtual void OnTriggerEnter2D(Collider2D collision)
   {
      var soap = collision.GetComponent<CollectableSoapController>();
      if (soap != null && !soap.IsCollectable)
      {
         var wasDirty = _hygieneComponent?.IsDirty ?? false;

         var hygieneConsumed = _hygieneComponent?.AddHygiene(soap.Hygiene) ?? 0f;
         soap.Hygiene -= hygieneConsumed;

         if (wasDirty && !(_hygieneComponent?.IsDirty ?? false))
         {
            Messaging.SendMessage(Enums.MessageType.NPC_CLEANED_BY_SOAP);
         }
      }

      var shower = collision.GetComponent<ShowerController>();
      if (shower != null && _hygieneComponent.IsDirty)
      {
         shower.UseShower(_inventoryComponent, _hygieneComponent, false);
      }
   }

   protected virtual void OnTriggerStay2D(Collider2D collision)
   {
      var puddle = collision.GetComponent<PuddleController>();
      if (puddle != null && _hygieneComponent != null)
      {
         //if the guy is dirty, make the water dirty at the global fixed rate
         // if the guy is clean but the water is dirty, make the guy dirty as same rate
         if (_hygieneComponent.IsDirty)
         {
            puddle.Hygiene -= Constants.HYGIENE_TRANSFER_RATE * Time.deltaTime;
         }
         else if (puddle.IsDirty)
         {
            _hygieneComponent.RemoveHygiene(Constants.HYGIENE_TRANSFER_RATE * Time.deltaTime);
         }
      }
   }
}

