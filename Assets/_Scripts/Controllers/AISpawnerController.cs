using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class AISpawnerController : MonoBehaviour
{
   private readonly float _minDelay = 1f;
   private readonly float _maxDelay = 3f;

   public AIType AITypeToSpawn;
   public float SpawnRate;

   private Run _run;

   private void OnEnable()
   {
      _run = Run.Every(Random.Range(_minDelay, _maxDelay), SpawnRate, SpawnAI);
   }

   private void OnDisable()
   {
      if (_run != null)
      {
         _run.Abort();
      }
   }

   private void SpawnAI()
   {
      AIManager.Instance.SpawnAI(AITypeToSpawn, transform.position);
   }

}
