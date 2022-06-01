using UnityEngine;

public class MonoBehaviourStatic<TSelfType> : MonoBehaviour where TSelfType : MonoBehaviour
{
   private static TSelfType _instance = null;

   public static TSelfType Instance
   {
      get
      {
         if (_instance == null)
         {
            _instance = (TSelfType)FindObjectOfType(typeof(TSelfType));

            if (_instance == null)
            {
               _instance = (new GameObject(typeof(TSelfType).Name)).AddComponent<TSelfType>();
            }

            //todo - disabled this since it doesnt work properly, and causes duplicates when scene is reloaded
            //DontDestroyOnLoad(_instance.gameObject);
         }

         return _instance;
      }
   }
}
