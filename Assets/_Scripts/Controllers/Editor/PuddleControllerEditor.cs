using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuddleController))]
public class PuddleControllerEditor : Editor
{
   private SerializedProperty _volumeChangedProp;
   //private SerializedProperty _volumeProp;

   private float _prevVolume;

   private void OnEnable()
   {
      _volumeChangedProp = serializedObject.FindProperty("_volumeChanged");
      //_volumeProp = serializedObject.FindProperty("_volume");
   }

   public override void OnInspectorGUI()
   {
      EditorGUI.BeginChangeCheck();
      DrawDefaultInspector();

      serializedObject.Update();

      if (EditorGUI.EndChangeCheck())
      {
         _volumeChangedProp.boolValue = true;
         EditorUtility.SetDirty(target);
      }

      serializedObject.ApplyModifiedProperties();
   }
}
