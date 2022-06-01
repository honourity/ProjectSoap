using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class SoapThrowController : AnimationEventBehaviour
{
   [SerializeField] private int _soapStaminaCost = 100;
   [SerializeField] private GameObject _soapPrefab = null;
   private float _throwDistance = 12f;
   
   private MovementController _movementController;
   private StaminaComponent _staminaComponent;
   private InventoryComponent _inventoryComponent;

   public bool CanThrowSoap()
   {
      return _staminaComponent.Stamina >= _soapStaminaCost;
   }

   private void Awake()
   {
      _movementController = GetComponent<MovementController>();
      _staminaComponent = GetComponent<StaminaComponent>();
      _inventoryComponent = GetComponent<InventoryComponent>();
   }

   private void ThrowSoap()
   {
      if (_staminaComponent != null)
      {
         var soaps = _inventoryComponent?.GetItemsOfType<SoapModel>();

         if (_staminaComponent.UseStamina(_soapStaminaCost) && soaps?.Count > 0)
         {
            //take a soap out of inventory
            var soap = soaps.LastOrDefault();
            _inventoryComponent?.RemoveItem(soap);

            //create the collectable soap
            GameObject soapInstance = Instantiate(_soapPrefab, transform.position, Quaternion.identity);
            var collectableSoapController = soapInstance.GetComponent<CollectableSoapController>();
            collectableSoapController.InitializeData(soap);

            var throwDirection = (_movementController.LastAttemptedMoveType == Enums.MoveType.IDLE_LAND || _movementController.LastAttemptedMoveType == Enums.MoveType.IDLE_WATER || _movementController.LastAttemptedMoveType == Enums.MoveType.NONE) ? _movementController.GetCurrentDirection().SnapTo4Way().ToVector() : _movementController.LastAttemptedMoveDirection.ToVector();
            var originOffset = transform.Find("SoapThrowAnchor")?.localPosition ?? Vector3.zero;
            var to = transform.position + ((Vector3)throwDirection.normalized * _throwDistance);

            //if soap might visually hit an npc as its travelling through air, autoaim so it will physically hit
            ProcessAutoAim(ref to, originOffset.y);

            StartCoroutine(collectableSoapController.SoapTrajectoryCoroutine(originOffset, to));
         }
      }
   }

   private void ProcessAutoAim(ref Vector3 to, float yOffset)
   {
      var hitTop = Physics2D.Raycast(transform.position + Vector3.up * yOffset, to - transform.position, Vector3.Distance(transform.position, to), LayerMask.GetMask(Constants.LAYER_AI));
      if (hitTop.collider != null)
      {
         to = to + Vector3.up * yOffset;
         return;
      }

      //if top raycast failed, do a 2nd mid-raycast just in case
      var hitMid = Physics2D.Raycast(transform.position + Vector3.up * (yOffset / 2f), to - transform.position, Vector3.Distance(transform.position, to), LayerMask.GetMask(Constants.LAYER_AI));
      if (hitMid.collider != null)
      {
         to = to + Vector3.up * (yOffset / 2f);
         return;
      }
   }

  

   #region ANIMATION EVENTS

   //throw soap started
   public override void AnimationEventBehaviour1()
   {
      ThrowSoap();
   }

   //throw soap finished
   public override void AnimationEventBehaviour2()
   {
      _movementController.SpecialMoveInProgress = false;
   }

   #endregion
}
