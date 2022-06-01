using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementController), typeof(Rigidbody2D), typeof(InventoryComponent))]
public class PlayerController : AnimationEventBehaviour
{
   public bool CanMove { get; private set; } = true;

   [SerializeField] private float _hygieneLostRate = 0.5f;
   [SerializeField] private float _balancingStaminaBurnRate = 100f;
   [SerializeField] private float _dashStaminaCost = 100f;
   [SerializeField] private float _dashVelocity = 50f;
   [SerializeField] private float _dashDuration = 0.1f;

   [Header("Sound Effects")]
   [SerializeField] private AudioClip _dashLandSound = null;
   [SerializeField] private AudioClip _dashWaterSound = null;

   private SoapThrowController _soapThrowingController;
   private MovementController _moveController;
   private HandController _handController;
   private InventoryComponent _inventoryComponent;
   private StaminaComponent _staminaComponent;
   private HygieneComponent _hygieneComponent;
   private MovementComponent _movementComponent;
   private Rigidbody2D _rigidBody;
   //private AudioSource _audioSource;

   private bool _dashing;
   private bool _inWater;

   private void Awake()
   {
      _soapThrowingController = GetComponent<SoapThrowController>();
      _moveController = GetComponent<MovementController>();
      _staminaComponent = GetComponent<StaminaComponent>();
      _rigidBody = GetComponent<Rigidbody2D>();
      _inventoryComponent = GetComponent<InventoryComponent>();
      _hygieneComponent = GetComponent<HygieneComponent>();
      _movementComponent = GetComponent<MovementComponent>();

      _handController = FindObjectOfType<HandController>();

      //_audioSource = gameObject.AddComponent<AudioSource>();
   }

   private void Update()
   {
      var hygieneRemoved = _hygieneComponent.RemoveHygiene(_hygieneLostRate * Time.deltaTime);
      if (!hygieneRemoved)
      {
         GameManager.Instance.GameOver();
      }
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      var soap = collision.GetComponent<CollectableSoapController>();
      if (soap != null && soap.IsCollectable)
      {
         if (soap.AttemptCollect()) _inventoryComponent?.AddItem(soap);
      }

      var shower = collision.GetComponent<ShowerController>();
      if (shower != null)
      {
         shower.UseShower(_inventoryComponent, _hygieneComponent);
      }

      var waterSource = collision.GetComponent<WaterSourceController>();
      if (waterSource != null)
      {
         _inWater = true;
         _movementComponent.RunSpeed = _movementComponent.OriginalWalkSpeed;
      }
   }

   private void OnTriggerStay2D(Collider2D collision)
   {
      var puddle = collision.GetComponent<PuddleController>();
      if (puddle != null)
      {
         if (puddle?.IsDirty ?? false)
         {
            _hygieneComponent.RemoveHygiene(Constants.HYGIENE_TRANSFER_RATE * Time.deltaTime);
         }
      }
   }

   private void OnTriggerExit2D(Collider2D collision)
   {
      var waterSource = collision.GetComponent<WaterSourceController>();
      if (waterSource != null)
      {
         _inWater = false;
         _movementComponent.RunSpeed = _movementComponent.OriginalRunSpeed;
      }
   }

   private void OnCollisionEnter2D(Collision2D collision)
   {
      var npc = collision.collider.GetComponent<AIBehaviour>();
      if (npc != null)
      {
         var npcMovementController = npc.GetComponent<MovementController>();
         if (npcMovementController != null && (_moveController?.CurrentMoveType == Enums.MoveType.DASH_LAND || _moveController?.CurrentMoveType == Enums.MoveType.DASH_WATER))
         {
            AttemptStun(npcMovementController, _moveController.GetCurrentDirection().ToVector());
         }
      }

      CancelDash();
   }

   public void Move(Enums.MoveType type, Vector2 direction)
   {
      if (CanMove)
      {
         //force movetype to at least be idle
         if (type == Enums.MoveType.NONE) type = Enums.MoveType.IDLE_LAND;

         if (_inWater && (type >= Enums.MoveType.IDLE_LAND && type < Enums.MoveType.IDLE_WATER))
         {
            //converting land moves to water moves if player is in water
            type += 4;
         }

         switch (type)
         {
            case Enums.MoveType.ATTACK:
               AttemptAttack(direction);
               break;
            case Enums.MoveType.DASH_WATER:
            case Enums.MoveType.DASH_LAND:
               AttemptDash(direction, type);
               break;
            default:
               _moveController.Move(type, direction);
               break;
         }
      }
   }

   public void BalanceSoap()
   {
      if (_staminaComponent.UseStamina(_balancingStaminaBurnRate * Time.deltaTime))
      {
         _handController.Center(10f);
      }
   }

   public void CancelDash()
   {
      _dashing = false;
   }

   private void AttemptAttack(Vector2 direction)
   {
      if (_soapThrowingController?.CanThrowSoap() ?? false)
      {
         _moveController.Move(Enums.MoveType.ATTACK, direction, _soapThrowingController);
      }
   }

   private void AttemptDash(Vector2 direction, Enums.MoveType type)
   {
      if (_staminaComponent?.UseStamina(_dashStaminaCost) ?? false)
      {
         if (_moveController.Move(type, direction, this))
         {
            CanMove = false;
            _moveController.enabled = false;

            //play sound depending land or water
            if (type == Enums.MoveType.DASH_LAND)
            {
               AudioManager.Instance.Play(_dashLandSound);
            }
            else if (type == Enums.MoveType.DASH_WATER)
            {
               AudioManager.Instance.Play(_dashWaterSound);
            }
         }
      }
   }

   private void AttemptStun(MovementController npcMovementController, Vector2 direction)
   {
      if (npcMovementController.CurrentMoveType != Enums.MoveType.STUNNED)
      {
         StopAllCoroutines();

         _rigidBody.velocity = Vector2.zero;
         CanMove = true;
         _moveController.SpecialMoveInProgress = false;
         CancelDash();
         _moveController.Move(Enums.MoveType.IDLE_LAND, direction);
         StartCoroutine(StunCoroutine());

         var shoveDistance = 3f;
         npcMovementController.Move(Enums.MoveType.STUNNED, direction.normalized * shoveDistance);

         //todo - start a screenshake coroutine in cameracontroller
      }
   }

   private IEnumerator StunCoroutine()
   {
      CanMove = false;

      yield return new WaitForSeconds(0.25f);

      //slowdown
      _movementComponent.RunSpeed = _movementComponent.OriginalWalkSpeed;
      CanMove = true;

      yield return new WaitForSeconds(0.5f);

      //speedup
      _movementComponent.RunSpeed = _movementComponent.OriginalRunSpeed;
   }

   private void OnEnable()
   {
      CanMove = true;
      CancelDash();
   }

   #region ANIMATION EVENTS

   //dash started
   public override void AnimationEventBehaviour1()
   {
      if (!_dashing) StartCoroutine(DashCoroutine());
   }

   private IEnumerator DashCoroutine()
   {
      CanMove = false;

      _dashing = true;

      var duration = _dashDuration;

      //emergency timeout for if something goes wrong
      // also a maximum dash duration under any circumstance
      var timeout = 2f;

      while (duration > 0f && timeout > 0f)
      {
         if (_dashing)
         {
            if (_inWater)
            {
               _moveController.Move(Enums.MoveType.DASH_WATER, _moveController.GetCurrentDirection());

               duration += Time.deltaTime;
            }
            else
            {
               _moveController.Move(Enums.MoveType.DASH_LAND, _moveController.GetCurrentDirection());
            }

            _rigidBody.velocity = _moveController.GetCurrentDirection().ToVector().normalized * _dashVelocity;

            yield return null;

            duration -= Time.deltaTime;
         }
         else
         {
            break;
         }

         timeout -= Time.deltaTime;
      }

      CanMove = true;

      CancelDash();
   }

   #endregion
}
