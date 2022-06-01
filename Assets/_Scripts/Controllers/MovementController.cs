using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D), typeof(MovementComponent))]
public class MovementController : AnimationEventBehaviour
{
   /// <summary>
   /// when this is false, the character is forced to Enums.MoveType.NONE (after they finish any special moves they might currently be in the middle of)
   /// </summary>
   [NonSerialized] public bool CanMove = true;

   public Enums.MoveType CurrentMoveType { get; private set; }
   public bool SpecialMoveInProgress { get; set; }
   public Enums.MoveType LastAttemptedMoveType { get; set; }
   public Enums.Direction LastAttemptedMoveDirection { get; set; }

   [SerializeField] private LayerMask _dashCollisionIgnore;
   private List<Animator> _animators = new List<Animator>();
   private Rigidbody2D _rigidBody;
   private HandController _handController;
   private MovementComponent _movementComponent;
   private AnimationEventBehaviour _lastAnimationEventBehaviour;
   private Collider2D _collider;

   public bool Move(Enums.MoveType type, Enums.Direction direction, AnimationEventBehaviour animationEventBehaviour = null)
   {
      return Move(type, direction.ToVector(), animationEventBehaviour);
   }

   public bool Move(Enums.MoveType type, Vector2 directionVector, AnimationEventBehaviour animationEventBehaviour = null)
   {
      var moveTaken = false;

      if (!SpecialMoveInProgress)
      {
         switch (type)
         {
            case Enums.MoveType.WALK_LAND:
            case Enums.MoveType.WALK_WATER:
               moveTaken = Walk(directionVector);
               break;
            case Enums.MoveType.RUN_LAND:
            case Enums.MoveType.RUN_WATER:
               moveTaken = Run(directionVector);
               break;
            case Enums.MoveType.DASH_LAND:
            case Enums.MoveType.DASH_WATER:
               moveTaken = Dash(directionVector);
               break;
            case Enums.MoveType.ATTACK:
               moveTaken = Attack(directionVector);
               break;
            case Enums.MoveType.FALL:
               moveTaken = Fall(directionVector);
               break;
            case Enums.MoveType.STUNNED:
               moveTaken = Stunned(directionVector);
               break;
            default:
               moveTaken = DefaultMove(directionVector);
               break;
         }

         if (moveTaken)
         {
            if (animationEventBehaviour == null) _lastAnimationEventBehaviour = this;

            _lastAnimationEventBehaviour = animationEventBehaviour;

            if (_animators.All(a => a.isActiveAndEnabled))
            {
               if (directionVector != Vector2.zero && directionVector.magnitude > 0) _animators.ForEach(a => a.SetFloat(Constants.MOVE_DIRECTION_KEY, (int)directionVector.ToCardinalDirection<Enums.Direction>()));
               _animators.ForEach(a => a.SetInteger(Constants.MOVE_TYPE_INT_KEY, (int)type));
               _animators.ForEach(a => a.SetFloat(Constants.MOVE_TYPE_FLOAT_KEY, a.GetInteger(Constants.MOVE_TYPE_INT_KEY)));
            }

            CurrentMoveType = type;
         }
      }

      LastAttemptedMoveType = type;
      LastAttemptedMoveDirection = directionVector.ToCardinalDirection<Enums.Direction>();

      return moveTaken;
   }

   public Enums.MoveType GetCurrentMove()
   {
      return (Enums.MoveType)_animators?.FirstOrDefault()?.GetInteger(Constants.MOVE_TYPE_INT_KEY);
   }

   public Enums.Direction GetCurrentDirection()
   {
      return (Enums.Direction)Mathf.RoundToInt(_animators?.FirstOrDefault()?.GetFloat(Constants.MOVE_DIRECTION_KEY) ?? 0);
   }

   private void Awake()
   {
      _rigidBody = GetComponent<Rigidbody2D>();
      _collider = GetComponent<Collider2D>();
      _movementComponent = GetComponent<MovementComponent>();

      //if this is a player, attempt to get the hand controller
      if (GetComponent<PlayerController>() != null)
      {
         _handController = FindObjectOfType<HandController>();
      }
   }

   private void Start()
   {
      //list of animators is to handle reflection sprites
      // this isnt in Awake for script execution order reasons (the reflection animations havnt been setup yet)
      _animators = GetComponentsInChildren<Animator>().ToList();
   }

   private bool DefaultMove(Vector2 direction)
   {
      _rigidBody.velocity = Vector2.zero;

      return true;
   }

   private bool Walk(Vector2 direction)
   {
      _rigidBody.velocity = direction.normalized * _movementComponent.WalkSpeed;
      _handController?.Balance(direction);

      return true;
   }

   private bool Run(Vector2 direction)
   {
      _rigidBody.velocity = direction.normalized * _movementComponent.RunSpeed;
      _handController?.Balance(direction);

      return true;
   }

   private bool Dash(Vector2 direction)
   {
      direction = GetCurrentDirection().ToVector().normalized;

      //fail the dash attempt if there is something blocking nearby
      var hits = Physics2D.RaycastAll(_collider.bounds.center, direction, _collider.bounds.extents.x + 0.05f, ~_dashCollisionIgnore);
      foreach (var hit in hits)
      {
         if (hit.collider != null && !hit.collider.isTrigger)
         {
            return false;
         }
      }

      _rigidBody.velocity = Vector2.zero;
      _handController?.Balance(direction);
      return true;
   }

   private bool Attack(Vector2 direction)
   {
      SpecialMoveInProgress = true;
      _rigidBody.velocity = Vector2.zero;
      _handController?.Balance(direction);

      return true;
   }

   private bool Fall(Vector2 direction)
   {
      _rigidBody.velocity = Vector2.zero;
      _handController?.Balance(direction);

      return true;
   }

   private bool Stunned(Vector2 direction)
   {
      _rigidBody.velocity = Vector2.zero;

      var knockbackVector = direction;
      var hit = Physics2D.Raycast(transform.position, direction, direction.magnitude, ~_dashCollisionIgnore);
      if (hit.collider != null)
      {
         knockbackVector = (direction.normalized * hit.distance);
      }

      CanMove = false;
      SpecialMoveInProgress = true;

      StartCoroutine(StunnedCoroutine(_rigidBody.position + knockbackVector));

      return true;
   }
   private IEnumerator StunnedCoroutine(Vector2 knockbackTargetPosition)
   {
      var knockbackRate = 10f;
      var timer = _movementComponent.StunnedDuration;

      while (timer > 0f)
      {
         _rigidBody.position = Vector2.Lerp(_rigidBody.position, knockbackTargetPosition, knockbackRate * Time.deltaTime);

         yield return null;

         timer -= Time.deltaTime;
      }

      CanMove = true;
      SpecialMoveInProgress = false;
   }

   #region GENERIC ANIMATION EVENTS

   public void AnimationEvent1()
   {
      _lastAnimationEventBehaviour?.AnimationEventBehaviour1();
   }

   public void AnimationEvent2()
   {
      _lastAnimationEventBehaviour?.AnimationEventBehaviour2();
   }

   public void AnimationEvent3()
   {
      _lastAnimationEventBehaviour?.AnimationEventBehaviour3();
   }

   public void AnimationEvent4()
   {
      _lastAnimationEventBehaviour?.AnimationEventBehaviour4();
   }

   public void AnimationEvent5()
   {
      _lastAnimationEventBehaviour?.AnimationEventBehaviour5();
   }

   #endregion
}
