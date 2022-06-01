using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[System.Serializable]
public class AITypePrefabLimitSettings
{
   public AIType Type;
   public GameObject Prefab;
   public int Limit = 10;
}

[System.Serializable]
public class AIInstanceList
{
   public AIType Type;
   public List<GameObject> Instances;

   public AIInstanceList(AIType type, GameObject newAI)
   {
      Type = type;
      Instances = new List<GameObject> { newAI };
   }
}

public class AIManager : MonoBehaviourStatic<AIManager>
{
   //private DoorComponent[] _doorPositions;
   public List<AITypePrefabLimitSettings> AITypeSettings;
   public List<AIInstanceList> AIInstances;

   private int numInstances;

   private void Awake()
   {
      //_doorPositions = FindObjectsOfType<DoorComponent>();
      AIInstances = new List<AIInstanceList>();
   }

   public GameObject GetPrefab(AIType type)
   {
      return AITypeSettings.Find(item => item.Type == type).Prefab;
   }

   public void SpawnAI(AIType type, Vector3 position)
   {
      //Check if we are over limit and make an AI leave the arena
      bool overLimit = CheckAndExitAI(type);

      if (!overLimit)
      {
         GameObject newAI = null;

         GameObject prefabToSpawn = AITypeSettings?.Find(setting => setting.Type == type)?.Prefab;
         if (prefabToSpawn != null) newAI = Instantiate(prefabToSpawn, position, Quaternion.identity);

         AIInstanceList aiInstanceList = AIInstances.Find(aiInstance => aiInstance.Type == type);

         if (newAI != null)
         {
            if (aiInstanceList != null)
            {
               aiInstanceList.Instances.Add(newAI);
            }
            else
            {
               AIInstanceList tempAIInstanceList = new AIInstanceList(type, newAI);
               AIInstances.Add(tempAIInstanceList);
            }
         }
      }
   }

   public bool CheckAndExitAI(AIType type)
   {
      AIInstanceList instanceList = AIInstances.Find(instance => instance.Type == type);
      AITypePrefabLimitSettings typeSetting = AITypeSettings.Find(setting => setting.Type == type);

      //CHECK IF WE ARE OVER THE LIMIT
      if (instanceList != null)
      {
         if (instanceList.Instances.Count >= typeSetting.Limit)
         {
            //Look for a clean AI to make exit
            for (int i = 0; i < instanceList.Instances.Count; i++)
            {
               if (!instanceList.Instances[i].GetComponent<HygieneComponent>().IsDirty)
               {
                  //Turn off all AIBehaviours before adding ExitAIModule, otherwise the brain
                  //will continue to think and switch modules at will
                  AIBehaviour[] behaviours = instanceList.Instances[i].GetComponents<AIBehaviour>();

                  if (behaviours != null && behaviours.Length > 0)
                  {
                     for(int j = 0; j < behaviours.Length; j++)
                     {
                        behaviours[j].enabled = false;
                     }
                  }

                  instanceList.Instances[i].AddComponent<ExitAIModule>();
                  instanceList.Instances.RemoveAt(i);
                  return false;
               }
            }

            //At this point we are over limit and there are no clean AIs to exit
            return true;
         }
      }

      //AT this point no instance List was found, so there are no AIs of this type yet
      return false;

   }
}
