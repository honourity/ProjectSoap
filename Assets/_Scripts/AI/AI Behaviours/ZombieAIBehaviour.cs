using UnityEngine;

public class ZombieAIBehaviour : AIBehaviour
{
   //AI MODULES
   private WanderingAIModule _wanderingModule;
   private PlayerChaserAIModule _playerChaserModule;
   private MonoBehaviour _currentAIModule;

   //INSPECTABLES
   [SerializeField] private float _pauseDuration = 2f;

   private float _distanceToPlayer;
   private bool _isDirty = true;

   protected override void Awake()
   {
      base.Awake();

      _wanderingModule = GetComponent<WanderingAIModule>() ?? gameObject.AddComponent<WanderingAIModule>();
      _playerChaserModule = GetComponent<PlayerChaserAIModule>() ?? gameObject.AddComponent<PlayerChaserAIModule>();

      _allAIModules.Add(_playerChaserModule);
      _allAIModules.Add(_wanderingModule);

   }

   protected override void OnEnable()
   {
      base.OnEnable();

      if (_currentAIModule != null) 
      {
         _currentAIModule.enabled = true;
      }
   }

   private void OnDestroy()
   {
      Destroy(_wanderingModule);
      Destroy(_playerChaserModule);
   }

   protected override void Start()
   {
      base.Start();

      _currentAIModule = _playerChaserModule;

      _wanderingModule.PauseDuration = _pauseDuration;
   }

   void Update()
   {
      ProcessBecameClean();
   }

   private void ProcessBecameClean()
   {
      if (_isDirty && (!_hygieneComponent?.IsDirty ?? false))
      {
         _isDirty = false;

         Debug.Log("zombie AI disable");
         enabled = false;

         //Random chance of the AI continuing to wander around 
         int randomNum = Random.Range(0, 2);

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
