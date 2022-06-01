using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerChaserAIModule : AIModule
{
   [HideInInspector] public float repathRate = 0.2f;

   //CACHED COMPONENTS
   private Seeker _seeker;
   private Rigidbody2D _rb2D;
   private MovementController _moveController;

   private Transform _player;
   private List<Vector3> _path;
   private Run _run;
   private Vector3 _direction = Vector3.zero;

   protected override void Awake()
   {
      _seeker = GetComponent<Seeker>();
      _rb2D = GetComponent<Rigidbody2D>();
      _player = GameManager.Instance.Player.transform;
      _moveController = GetComponent<MovementController>();
   }

   protected override void OnEnable()
   {
      base.OnEnable();

      if (_player == null) enabled = false;

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

      //The Tag of the node where the player is currently standing on.
      //We add this value by 1 since the first tag is 0, but is represented with bitmask 1. 
      //Bitmask containing tag 0 and tag 1 is 3 (in binary: 11) 
      int currPlayerPosNodeTag = (int)AstarPath.active.GetNearest(_player.position).node.Tag+1;
      bool traversable = ((_seeker.traversableTags & currPlayerPosNodeTag) == currPlayerPosNodeTag);

      if (_player != null && _player.gameObject.activeInHierarchy && traversable)
      {
         _seeker?.StartPath(transform.position, _player.position, OnPathComplete);
      }
   }

   private void OnPathComplete(Path path)
   {
      if (path.error)
      {
         Debug.LogWarning(name + " could not find a path to the player");
         return;
      }
      else
      {
         _path = path.vectorPath;
      }


      StartCoroutine(PathfindingUtility.FollowPath(_path, transform, _rb2D, _moveController, Enums.MoveType.RUN_LAND, OnCompleteFollowPath));
   }

   private void OnCompleteFollowPath(bool success)
   {
      _rb2D.velocity = Vector2.zero;
   }
}
