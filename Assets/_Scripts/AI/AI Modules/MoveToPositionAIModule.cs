using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MoveToPositionAIModule : AIModule
{
   private enum MoveToPositionMode { FollowTransform, PositionOnly }

   public override bool IsExclusive
   {
      get
      {
         return false;
      }
   }

   [SerializeField] private MoveToPositionMode _mode = MoveToPositionMode.FollowTransform;
   [SerializeField] private Transform _targetToFollow;
   [SerializeField] private Vector3 _positionToGo;

   [HideInInspector] public float repathRate = 0.2f;

   //CACHED COMPONENTS
   private Seeker _seeker;
   private Rigidbody2D _rb2D;
   private MovementController _moveController;

   private List<Vector3> _path;
   private Run _run;
   private Vector3 _direction = Vector3.zero;

   protected override void Awake()
   {
      _seeker = GetComponent<Seeker>();
      _rb2D = GetComponent<Rigidbody2D>();
      _moveController = GetComponent<MovementController>();
   }

   protected override void OnEnable()
   {
      base.OnEnable();

      _run = Run.Every(0.1f, repathRate, SearchPath);

   }

   private void OnDisable()
   {
      _run?.Abort();
      StopAllCoroutines();
      _rb2D.velocity = Vector2.zero;

      if (gameObject.activeInHierarchy)  //Need this checks, or warnings when leaving Play Mode
         _moveController?.Move(Enums.MoveType.IDLE_LAND, _direction);
   }

   private void SearchPath()
   {
      StopAllCoroutines();

      Vector3 actualTarget = Vector3.positiveInfinity;

      if (_mode == MoveToPositionMode.FollowTransform)
      {
         actualTarget = _targetToFollow.position;
      }
      else
      {
         actualTarget = _positionToGo;
      }

      _seeker?.StartPath(transform.position, actualTarget, OnPathComplete);
   }

   private void OnPathComplete(Path path)
   {
      if (path.error)
      {
         Debug.LogWarning(name + " could not find a path to the player");
         return;
      }
      else
         _path = path.vectorPath;

      StartCoroutine(PathfindingUtility.FollowPath(_path, transform, _rb2D, _moveController, Enums.MoveType.RUN_LAND, OnCompleteFollowPath));
   }

   private void OnCompleteFollowPath(bool success)
   {
      Debug.Log("Reached the target");
      _run.Abort();
      _rb2D.velocity = Vector2.zero;
   }

}
