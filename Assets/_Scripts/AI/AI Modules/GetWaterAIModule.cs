using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// Searches for the closest water source and moves the AI there to replenish its water source
/// </summary>
public class GetWaterAIModule : AIModule
{
   //CACHED COMPONENTS
   private Seeker _seeker;
   private Rigidbody2D _rb2D;
   private MovementController _moveController;

   [HideInInspector] public float repathRate = 1.5f;

   private List<Vector3> _path;
   private Coroutine _pathFollowCoroutine;

   private bool hasStarted = false;

   protected override void Awake()
   {
      base.Awake();

      _seeker = GetComponent<Seeker>();
      _rb2D = GetComponent<Rigidbody2D>();
      _moveController = GetComponent<MovementController>();
   }

   protected override void OnEnable()
   {
      base.OnEnable();

      if (hasStarted)
      {
         Start();
      }
   }

   private void Start()
   {
      GoToClosestWater();
      hasStarted = true;
   }

   private void OnDisable()
   {
      if (_pathFollowCoroutine != null)
      {
         StopCoroutine(_pathFollowCoroutine);
         _rb2D.velocity = Vector2.zero;

         if (gameObject.activeInHierarchy)
         {
            _moveController.Move(Enums.MoveType.IDLE_LAND, _rb2D.velocity.normalized);
         }
      }
   }

   private void GoToClosestWater()
   {
      NNConstraint constraint = new NNConstraint();
      constraint.constrainWalkability = true;
      constraint.walkable = true;

      constraint.constrainTags = true;
      constraint.tags = (1 << 1);

      NNInfo info = AstarPath.active.GetNearest(transform.position, constraint);

      _seeker.StartPath(transform.position, info.position, path =>
      {
         _pathFollowCoroutine = StartCoroutine(PathfindingUtility.FollowPath(path.vectorPath, transform, _rb2D, _moveController, Enums.MoveType.RUN_LAND, success =>
         {
            if (!success)
            {
               GoToClosestWater();
            }
         }));

         _seeker.ReleaseClaimedPath();
         _seeker.lastCompletedVectorPath = path.vectorPath;
      });
   }
}
