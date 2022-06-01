using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CompositeColliderChildrenController))]
public class CompositeColliderChildrenControllerEditor : Editor
{
   CompositeColliderChildrenController cccc;

   private void OnEnable()
   {
      cccc = (CompositeColliderChildrenController)target;
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();

      if (GUILayout.Button("Disable Using Composite"))
      {
         cccc.DisableUsingComposite();
      }
      
      if (GUILayout.Button("Enable Using Composite"))
      {
         cccc.EnableUsingComposite();
      }
   }
}
