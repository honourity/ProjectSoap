using UnityEngine;
using Pathfinding;

public class ExitAIModule : AIModule
{
   //CACHED COMPONENTS
   private Seeker _seeker;
   private Rigidbody2D _rb2D;
   private MovementController _moveController;

   private DoorComponent[] _allDoors;
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
      base.OnEnable();
      hasStarted = true;
      ExitToClosestDoor();
   }

   private void ExitToClosestDoor()
   {
      _allDoors = FindObjectsOfType<DoorComponent>();

      if (_allDoors != null && _allDoors.Length > 0)
      {
         //Search for the closest door
         StartCoroutine(PathfindingUtility.StartShortestPath(_allDoors, transform, _seeker, shortestPath =>
         {
            _seeker.ReleaseClaimedPath();
            _seeker.lastCompletedVectorPath = shortestPath.vectorPath;

            //Begin following the path to the closest door
            StartCoroutine(PathfindingUtility.FollowPath(shortestPath.vectorPath, transform, _rb2D, _moveController, Enums.MoveType.WALK_LAND, success =>
            {
               //Check if we succeeded reaching the door or not?

               if (success)
               {
                  _moveController.Move(Enums.MoveType.IDLE_LAND, _rb2D.velocity.normalized);
                  Destroy(gameObject, 1.5f);
               }
               else
               {
                  ExitToClosestDoor();
               }
            }));
         }));

      }
   }

}
