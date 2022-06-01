using UnityEngine;
using Pathfinding;
using System;
using System.Linq;

[RequireComponent(typeof(MovementController), typeof(Seeker), typeof(DripController))]
public class DrippyAIBehaviour : AIBehaviour
{
   private enum DrippyAIState
   {
      None,      //Do nothing
      Wandering, //has water, and is dripping
      GoToWater //run out of water, looking for water to refill
   }

   [SerializeField] private DrippyAIState _state;
   [SerializeField] private int _maxWater = 300;

   //CACHED COMPONENTS
   private InventoryComponent _inventory;
   private DripController _dripController;
   private Collider2D _collider;
   private Seeker _seeker;


   //AI MODULES
   private WanderingAIModule _wanderingModule;
   private GetWaterAIModule _getWaterModule;

   private bool _wasDirty = true;

   protected override void Awake() 
   {
      base.Awake();

      //GET COMPONENTS
      _inventory = GetComponent<InventoryComponent>();
      _seeker = GetComponent<Seeker>();

      _getWaterModule = GetComponent<GetWaterAIModule>() ?? gameObject.AddComponent<GetWaterAIModule>();
      _wanderingModule = GetComponent<WanderingAIModule>() ?? gameObject.AddComponent<WanderingAIModule>();
      _dripController = GetComponent<DripController>();
      _collider = GetComponent<Collider2D>();

      _allAIModules.Add(_getWaterModule);
      _allAIModules.Add(_wanderingModule);
   }

   private void Update()
   {
      if (_hygieneComponent?.IsDirty ?? false)
      {
         var water = _inventory?.GetItemsOfType<WaterModel>()?.FirstOrDefault();

         if (water?.Volume > 1 && _state != DrippyAIState.Wandering)
         {
            //Begin Wandering
            _seeker.traversableTags = 1 << 0;
            _state = DrippyAIState.Wandering;
            _wanderingModule.PauseDuration = UnityEngine.Random.Range(1f, 5f);
            _wanderingModule.enabled = true;
         }
         else if ((water == null || water?.Volume <= 1) && _state != DrippyAIState.GoToWater)
         {
            //Search for water
            _seeker.traversableTags = 1 << 0 | 1 << 1;
            _state = DrippyAIState.GoToWater;
            _getWaterModule.enabled = true;
         }
      }

      ProcessBecameClean();
   }

   protected override void OnTriggerEnter2D(Collider2D collision)
   {
      base.OnTriggerEnter2D(collision);

      var waterSource = collision.GetComponent<WaterSourceController>();
      if (waterSource != null)
      {
         //var _dirtyComponent = waterSource.GetComponent<DirtyComponent>();
         var water = _inventory?.GetItemsOfType<WaterModel>()?.FirstOrDefault();

         if (water == null)
         {
            water = new WaterModel(Guid.NewGuid(), _maxWater);
            _inventory?.AddItem(water);
         }
         else
         {
            water.AddVolume(Mathf.Max(0f, _maxWater - water.Volume));
         }
      }
   }

   private void ProcessBecameClean()
   {
      if (_wasDirty && (!_hygieneComponent?.IsDirty ?? false))
      {
         _wasDirty = false;

         enabled = false;
         _dripController.enabled = false;
         _collider.isTrigger = false;

         //Random chance of the AI continuing to wander around
         int randomNum = UnityEngine.Random.Range(0, 2);
         if (randomNum >= 1)
         {
            Run.After(1f, () =>
            {
               enabled = true;
               _wanderingModule.enabled = true;
            });
         }
      }
   }
}
