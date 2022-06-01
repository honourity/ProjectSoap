using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AstarScanner))]
public class AstarScannerEditor : Editor
{
   AstarScanner aStarScanner;

   void OnEnable()
   {
      aStarScanner = (AstarScanner)target;
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();

      if (GUILayout.Button("Custom Scan"))
      {
         aStarScanner.DoScan();
      }
   }
}
