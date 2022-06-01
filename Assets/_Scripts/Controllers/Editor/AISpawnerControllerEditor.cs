using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AISpawnerController))]
public class AISpawnerControllerEditor : Editor
{

   private void OnEnable()
   {
      
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
   }
}
