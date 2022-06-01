using UnityEngine;
using Pathfinding;
public class FatAIBehaviour : AIBehaviour
{
   private enum FatAIState { Wandering, Chasing }

   private FatAIState _state;

   //AI MODULES
   private WanderingAIModule _wanderingModule;
   private PlayerChaserAIModule _playerChaserModule;

   //INSPECTABLES
   [SerializeField] private float _radius = 3f;
   [SerializeField] private float _chaseRadiusMultiplier = 2.5f;
   [SerializeField] Vector2 _radiusOffset = Vector2.zero;
   [SerializeField] private float _pauseDuration = 2f;

   //CACHED COMPONENT
   private Seeker _seeker;

   private Transform _player;
   private Vector2 radiusCenter;
   private float _distanceToPlayer;
   private float _initialRadius;

   private bool _isDirty = true;

   protected override void Awake()
   {
      base.Awake();
      _seeker = GetComponent<Seeker>();

      _playerChaserModule = GetComponent<PlayerChaserAIModule>() ?? gameObject.AddComponent<PlayerChaserAIModule>();
      _wanderingModule = GetComponent<WanderingAIModule>() ?? gameObject.AddComponent<WanderingAIModule>();
      _allAIModules.Add(_playerChaserModule);
      _allAIModules.Add(_wanderingModule);
      _player = GameManager.Instance.Player.transform;
   }

   protected override void OnEnable()
   {
      base.OnEnable();

      if (_currentModule != null) _currentModule.enabled = true;
   }

   private void OnDestroy()
   {
      Destroy(_wanderingModule);
      Destroy(_playerChaserModule);
   }

   protected override void Start()
   {
      base.Start();

      //Set the traversable tags to being tag 0 (Basic Ground) only
      _seeker.traversableTags = 1 << 0;
      _initialRadius = _radius;
      _currentModule = _wanderingModule;
      _playerChaserModule.enabled = false;

      _wanderingModule.PauseDuration = _pauseDuration;
   }

   void Update()
   {
      if (!IgnorePlayer && (_hygieneComponent?.IsDirty ?? false))
      {
         radiusCenter = (Vector2)transform.position + _radiusOffset;
         if (_player != null) _distanceToPlayer = Vector2.Distance(radiusCenter, _player.position);

         int currPlayerPosNodeTag = (int)AstarPath.active.GetNearest(_player.position).node.Tag + 1;
         bool traversable = ((_seeker.traversableTags & currPlayerPosNodeTag) == currPlayerPosNodeTag);

         if (_state != FatAIState.Chasing && traversable && _distanceToPlayer <= _radius && _player != null && _player.gameObject.activeInHierarchy)
         {
            _state = FatAIState.Chasing;
            _playerChaserModule.enabled = true;
            _currentModule = _playerChaserModule;
            _radius = _initialRadius * _chaseRadiusMultiplier;
         }
         else if ((_distanceToPlayer > _radius || !traversable) && _state != FatAIState.Wandering)
         {
            _state = FatAIState.Wandering;
            _wanderingModule.enabled = true;
            _currentModule = _wanderingModule;
            _radius = _initialRadius;
         }
      }
      else if (IgnorePlayer)
      {
         _wanderingModule.enabled = true;
         _currentModule = _wanderingModule;
         _radius = _initialRadius;
      }

      ProcessBecameClean();
   }

   private void OnDrawGizmos()
   {
      radiusCenter = (Vector2)transform.position + _radiusOffset;
      DebugExtension.DrawPoint(radiusCenter, Color.yellow, 0.25f);
      DebugExtension.DrawCircle(radiusCenter, Vector3.back, Color.yellow, _radius);
   }

   public void ResetRadius()
   {
      _radius = _initialRadius;
   }

   public void SetRadius(float radius)
   {
      _radius = radius;
   }

   private void ProcessBecameClean()
   {
      if (_isDirty && (!_hygieneComponent?.IsDirty ?? false))
      {
         _isDirty = false;

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
