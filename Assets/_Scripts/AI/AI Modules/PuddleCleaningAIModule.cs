using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using Pathfinding;

public class PuddleCleaningAIModule : AIModule
{
   [HideInInspector] public PuddleController[] AllPuddles;
   [HideInInspector] public PuddleController CurrentPuddle;
   public float MopRate = 5f;
   public float MaxWater = 100f;

   public Action OnNoMorePuddles;
   public Action OnWaterFilled;

   //CACHED COMPONENTS
   private Seeker _seeker;
   private InventoryComponent _inventory;
   private MovementController _moveController;
   private Rigidbody2D _rb2D;

   private WaterModel _water;
   private bool _hasStarted = false;

   protected override void Awake()
   {
      _seeker = GetComponent<Seeker>();
      _inventory = GetComponent<InventoryComponent>();
      _rb2D = GetComponent<Rigidbody2D>();
      _moveController = GetComponent<MovementController>();
   }

   protected override void OnEnable()
   {
      base.OnEnable();

      //Pathfinding needs to happen in Start since it's not ready until then
      if (_hasStarted)
      {
         Start();
      }
   }

   private void Start()
   {
      _water = _inventory?.GetItemsOfType<WaterModel>()?.FirstOrDefault();

      if (_water == null)
      {
         _water = new WaterModel(Guid.NewGuid(), 0f);
         _inventory?.AddItem(_water);
      }

      StartCoroutine(GoToPuddle());
   }

   private void OnDisable()
   {
      StopAllCoroutines();
      AllPuddles = null;
      CurrentPuddle = null;
   }

   private void Update()
   {
      if (CurrentPuddle != null)
      {
         if (CurrentPuddle.Volume > 0f)
         {
            float waterValue = MopRate * Time.deltaTime;

            CurrentPuddle.Volume -= waterValue;
            _water.AddVolume(Mathf.Max(0f, waterValue));

            if (_water.Volume > MaxWater)
            {
               _moveController.Move(Enums.MoveType.IDLE_LAND, _moveController.GetCurrentDirection());
               OnWaterFilled?.Invoke();
               CurrentPuddle = null;
            }
         }
         else //Done cleaning
         {
            //TODO 
            //FIX inaccuracy of adding water volume

            Destroy(CurrentPuddle.gameObject);
            CurrentPuddle = null;
            StartCoroutine(GoToPuddle());
         }
      }
   }

   private IEnumerator GoToPuddle()
   {
      //Wait a frame so any puddle destruction can finish first
      yield return null;

      //Find the biggest Puddle
      AllPuddles = FindObjectsOfType<PuddleController>();

      //Are there any puddles?
      if (AllPuddles == null || AllPuddles.Length <= 0)
      {
         OnNoMorePuddles?.Invoke();
         yield break;
      }

      float biggestVolume = 0;
      int biggestIndex = 0;

      for (int i = 0; i < AllPuddles.Length; i++)
      {
         if (AllPuddles[i].Volume > biggestVolume)
         {
            biggestVolume = AllPuddles[i].Volume;
            biggestIndex = i;
         }
      }

      PuddleController puddle = AllPuddles[biggestIndex];

      Path path = _seeker.StartPath(transform.position, puddle.transform.position);
      yield return StartCoroutine(path.WaitForPath());

      StartCoroutine(PathfindingUtility.FollowPath(path.vectorPath, transform, _rb2D, _moveController, Enums.MoveType.RUN_LAND, success =>
      {
         if (success)
         {
            CurrentPuddle = puddle;
            _moveController.Move(Enums.MoveType.CLEANING, _moveController.GetCurrentDirection());
         }
      }));
   }


}
