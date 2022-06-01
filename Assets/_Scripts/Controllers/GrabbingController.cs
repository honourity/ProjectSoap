using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class GrabbingController : AnimationEventBehaviour
{
   [SerializeField]
   private float _grabRadius = 1f;
   [SerializeField]
   private float _grabCooldown = 2f;
   [SerializeField]
   private float _grabDuration = 1f;
   [SerializeField]
   private float _hygieneRemoved = 25f;

   private MovementController _movementController;
   private SpriteRenderer _spriteRenderer;
   private Collider2D _collider;
   private AIBehaviour _aiBehaviour;
   private HygieneComponent _hygieneComponent;
   private bool _colliderWasTrigger;
   private MovementController _grabeeCached;
   private bool _grabeeFound;
   private bool _grabbing;
   private float _grabCooldownProgress;

   private void Awake()
   {
      _collider = GetComponent<Collider2D>();
      _movementController = GetComponent<MovementController>();
      _aiBehaviour = GetComponent<AIBehaviour>();
      _hygieneComponent = GetComponent<HygieneComponent>();
      _spriteRenderer = GetComponent<SpriteRenderer>();
   }

   private void Update()
   {
      if (_grabCooldownProgress > 0f)
      {
         _grabCooldownProgress -= Time.deltaTime;
      }
      else
      {
         if (_aiBehaviour != null) _aiBehaviour.IgnorePlayer = false;
      }

      _grabeeFound = false;

      if (_hygieneComponent.IsDirty && !_grabbing && _grabCooldownProgress <= 0f)
      {
         var hits = Physics2D.OverlapCircleAll(transform.position, _grabRadius);
         foreach (var hit in hits)
         {
            var grabeePlayerController = hit.GetComponent<PlayerController>();
            var grabeeMovementController = hit.GetComponent<MovementController>();
            if (grabeePlayerController != null
               && grabeeMovementController != null
               && grabeeMovementController != _movementController
               && grabeeMovementController.CurrentMoveType != Enums.MoveType.DASH_LAND
               && grabeeMovementController.CurrentMoveType != Enums.MoveType.DASH_WATER)
            {
               _grabeeCached = grabeeMovementController;
               _grabeeFound = true;

               break;
            }
         }

         if (_grabeeFound && _grabeeCached != null
            && _movementController.CanMove
            && !_grabeeCached.SpecialMoveInProgress
            && !_movementController.SpecialMoveInProgress)
         {
            if (_aiBehaviour != null)
            {
               _aiBehaviour.IgnorePlayer = true;
            }

            _movementController.Move(Enums.MoveType.GRAB, Enums.Direction.EAST, this);
            _grabeeCached.Move(Enums.MoveType.GRABBED, Enums.Direction.WEST, this);

            _grabCooldownProgress = float.MaxValue;

            //disable everything
            _movementController.SpecialMoveInProgress = true;
            _grabeeCached.SpecialMoveInProgress = true;
         }
      }
   }

   private void OnDisable()
   {
      if (_aiBehaviour != null) _aiBehaviour.IgnorePlayer = false;
   }

   #region ANIMATION EVENTS

   //grab started
   public override void AnimationEventBehaviour1()
   {
      if (!_grabbing && _grabeeCached != null && _movementController != null)
      {
         

         if (!_grabbing) StartCoroutine(GrabCoroutine());
      }
   }

   //falling started for grabee
   public override void AnimationEventBehaviour2()
   {
      _grabeeCached.CanMove = false;
      _grabeeCached.SpecialMoveInProgress = true;
   }

   //falling finished for grabee
   public override void AnimationEventBehaviour3()
   {
      _grabeeCached.CanMove = true;
      _grabeeCached.SpecialMoveInProgress = false;
      _grabeeCached.Move(Enums.MoveType.IDLE_LAND, Enums.Direction.WEST, this);

      _collider.isTrigger = _colliderWasTrigger;
   }

   private IEnumerator GrabCoroutine()
   {
      var originalGrabeePosition = _grabeeCached.transform.position;
      var originalPosition = _movementController.transform.position;

      var middle = ((_grabeeCached.transform.position - _movementController.transform.position) / 2f) + _movementController.transform.position;
      _grabeeCached.transform.position = middle;
      _movementController.transform.position = middle;

      _grabbing = true;

      _colliderWasTrigger = _collider.isTrigger;
      _collider.isTrigger = true;

      //remove hygiene
      var grabeeHygiene = _grabeeCached.GetComponent<HygieneComponent>();
      grabeeHygiene?.RemoveHygiene(_hygieneRemoved);

      yield return new WaitForSeconds(_grabDuration);

      _grabCooldownProgress = _grabCooldown;

      //enable everything
      _movementController.SpecialMoveInProgress = false;
      _grabeeCached.CanMove = true;
      _grabeeCached.SpecialMoveInProgress = false;

      //make the grabee fall down
      _grabeeCached.Move(Enums.MoveType.FALL, Enums.Direction.WEST, this);

      //_collider.isTrigger = _colliderWasTrigger; //this has been moved till after player has finished falling
      _grabbing = false;

      //force instant idle after finishing the grab
      _movementController.Move(Enums.MoveType.IDLE_LAND, _movementController.GetCurrentDirection());

      //hack - since grab animation is roughly 2 units wide sprite with middle anchor
      // and normal sprites are 1 unit wide with middle anchor, offset after grab,
      // to roughly align with grab finish position
      var grabeeSpriteRenderer = _grabeeCached.GetComponent<SpriteRenderer>();
      _grabeeCached.transform.position += Vector3.right * (grabeeSpriteRenderer.bounds.extents.y / 2f);
      _movementController.transform.position -= Vector3.right * (_spriteRenderer.bounds.extents.y / 2f);
   }

   #endregion
}
