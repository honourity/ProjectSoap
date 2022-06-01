using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WanderingAIModule : AIModule
{
   [HideInInspector] public float PauseDuration = 0f;
   [HideInInspector] public float RepathRate = 0.5f;

   //CACHED COMPONENTS
   private Seeker _seeker;
   private Rigidbody2D _rb2D;
   private MovementController _moveController;

   private List<Vector3> _path;
   private Run _repathRun;
   private Vector2 _destination;
   private Run _newDestinationRun;
   private Coroutine _followPathCoroutine;

   protected override void Awake()
   {
      _seeker = GetComponent<Seeker>();
      _rb2D = GetComponent<Rigidbody2D>();
      _moveController = GetComponent<MovementController>();

   }

   protected override void OnEnable()
   {
      base.OnEnable();
      _newDestinationRun = Run.After(PauseDuration, NewDestination);
   }

   private void OnDisable()
   {
      if (_repathRun != null)
      {
         _repathRun.Abort();
      }

      if (_newDestinationRun != null)
      {
         _newDestinationRun.Abort();
      }

      if (_followPathCoroutine != null)
      {
         StopCoroutine(_followPathCoroutine);
      }

      _seeker.ReleaseClaimedPath();

      //Removes the path gizmo
      _seeker.lastCompletedVectorPath = null;

      _rb2D.velocity = Vector2.zero;

      if (gameObject.activeInHierarchy)   //Need these checks, or warnings when leaving Play Mode
      {
         _moveController.Move(Enums.MoveType.IDLE_LAND, _rb2D.velocity.normalized);
      }

   }

   private void NewDestination()
   {
      _destination = PathfindingManager.Instance.GetRandomWalkablePosition(PathfindingUtility.GetTagNumsByNames(new string[] { "Bath" }));
      _repathRun = Run.Every(0, RepathRate, SearchPath);
   }

   private void SearchPath()
   {
      _seeker.StartPath(transform.position, _destination, OnPathComplete);
   }

   private void OnPathComplete(Path path)
   {
      if (path.error)
      {
         Debug.LogWarning(name + " could not find a path to the target");
         return;
      }
      else
      {
         _path = path.vectorPath;
         if (_followPathCoroutine != null)
         {
            StopCoroutine(_followPathCoroutine);
         }

         if (enabled)
         {
            _followPathCoroutine = StartCoroutine(PathfindingUtility.FollowPath(_path, transform, _rb2D, _moveController, Enums.MoveType.WALK_LAND, OnCompletePathFollow));
         }
      }
   }

   private void OnCompletePathFollow(bool success)
   {
      //By this point we've reached the target, so stop moving
      _rb2D.velocity = Vector2.zero;
      _moveController.Move(Enums.MoveType.IDLE_LAND, _rb2D.velocity.normalized);
      _repathRun.Abort();
      _seeker.ReleaseClaimedPath();

      //Pause for a time and then go somewhere else
      _newDestinationRun = Run.After(PauseDuration, NewDestination);
   }

}
