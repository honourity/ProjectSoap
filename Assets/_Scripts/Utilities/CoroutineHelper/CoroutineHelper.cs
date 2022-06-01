using System.Collections.Generic;

public class CoroutineHelper : MonoBehaviourStatic<CoroutineHelper>
{
   private List<Run> m_OnGUIObjects = new List<Run>();
   public int ScheduledOnGUIItems
   {
      get { return m_OnGUIObjects.Count; }
   }

   public Run Add(Run aRun)
   {
      if (aRun != null)
         m_OnGUIObjects.Add(aRun);
      return aRun;
   }

   private void OnGUI()
   {
      for (var i = 0; i < m_OnGUIObjects.Count; i++)
      {
         var r = m_OnGUIObjects[i];
         if (!r.abort && !r.isDone && r.onGUIaction != null)
            r.onGUIaction();
         else
            r.isDone = true;
      }
   }

   private void Update()
   {
      for (int i = m_OnGUIObjects.Count - 1; i >= 0; i--)
      {
         if (m_OnGUIObjects[i].isDone)
            m_OnGUIObjects.RemoveAt(i);
      }
   }
}
