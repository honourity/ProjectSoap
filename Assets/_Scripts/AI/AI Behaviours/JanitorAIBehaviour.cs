using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(MovementController), typeof(Seeker))]
public class JanitorAIBehaviour : AIBehaviour
{
   [SerializeField] private float _mopRate = 5f;
   [SerializeField] private float _maxWater = 100f;

   //AI MODULES
   private PuddleCleaningAIModule _puddleCleanModule;
   private ExitAIModule _exitModule;

   private PuddleController[] _allPuddles;

   protected override void Awake() 
   {
      base.Awake();

      //GET COMPONENTS
      _hygieneComponent = GetComponent<HygieneComponent>();

      //CREATE MODULES
      _exitModule = GetComponent<ExitAIModule>() ?? gameObject.AddComponent<ExitAIModule>();
      _puddleCleanModule = GetComponent<PuddleCleaningAIModule>() ?? gameObject.AddComponent<PuddleCleaningAIModule>();

      //ADD MODULES TO LIST
      _allAIModules.Add(_exitModule);
      _allAIModules.Add(_puddleCleanModule);


   }

   protected override void OnEnable()
   {
      base.OnEnable();
      if (_currentModule != null)
      {
         _currentModule.enabled = true;
      }

      _puddleCleanModule.OnNoMorePuddles += ExitAI;
      _puddleCleanModule.OnWaterFilled += ExitAI;
      _puddleCleanModule.MopRate = _mopRate;
      _puddleCleanModule.MaxWater = _maxWater;

   }

   protected override void OnDisable()
   {
      base.OnDisable();

      _puddleCleanModule.OnNoMorePuddles -= ExitAI;
      _puddleCleanModule.OnWaterFilled -= ExitAI;
   }

   private void ExitAI()
   {
      _exitModule.enabled = true;
   }
}
