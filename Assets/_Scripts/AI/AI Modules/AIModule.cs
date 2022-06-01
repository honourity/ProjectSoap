using UnityEngine;

public abstract class AIModule : MonoBehaviour
{
   /// <summary>
   /// If true, this module cannot run alongside other modules
   /// </summary>
   public virtual bool IsExclusive { get { return true; } set { IsExclusive = value; } }

   protected virtual void Awake()
   {
      //Only hide if there's an AIBehaviour also attached to the gameObject
      AIBehaviour behaviour = GetComponent<AIBehaviour>();
      if (behaviour != null)
      {
         hideFlags = HideFlags.HideInInspector;
      }
   }

   protected virtual void OnEnable()
   {
      //Disable all other AIModules if this is exclusive
      if (IsExclusive)
      {
         AIModule[] allAIModule = GetComponents<AIModule>();

         for (int i = 0; i < allAIModule.Length; i++)
         {
            if (allAIModule[i] != this)
            {
               allAIModule[i].enabled = false;
            }
         }
      }
   }
}
