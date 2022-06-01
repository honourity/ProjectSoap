using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class SoapSpawnManager : MonoBehaviourStatic<SoapSpawnManager>
{
   [SerializeField] private GameObject _soapPrefab = null;
   [SerializeField] private int _maxSoap = 5;
   [SerializeField] private float _soapSpawnDelay = 5f;

   public List<Transform> AllSoapTransforms;

   private Run _soapSpawnRun;

   private void Awake()
   {
      AllSoapTransforms = new List<Transform>();
      AllSoapTransforms.Capacity = _maxSoap;
   }

   private void OnEnable()
   {
      Messaging.AddListener<CollectableSoapController>(Enums.MessageType.SOAP_COLLECTABLE_DESTROYED, OnSoapDestroyed);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener<CollectableSoapController>(Enums.MessageType.SOAP_COLLECTABLE_DESTROYED, OnSoapDestroyed);
      _soapSpawnRun.Abort();
   }

   private void OnSoapDestroyed(CollectableSoapController s)
   {
      AllSoapTransforms.Remove(s.transform);
   }

   private void Start()
   {
      _soapSpawnRun = Run.Every(1f, _soapSpawnDelay, SpawnSoap);
   }

   private void SpawnSoap()
   {
      if (AllSoapTransforms.Count < _maxSoap)
      {
         Vector3 randomPos = PathfindingManager.Instance.GetRandomWalkablePosition(PathfindingUtility.GetTagNumsByNames(new string[] { "Bath" }));
         AllSoapTransforms.Add(Instantiate(_soapPrefab, randomPos, Quaternion.identity).transform);
      }
   }
}
