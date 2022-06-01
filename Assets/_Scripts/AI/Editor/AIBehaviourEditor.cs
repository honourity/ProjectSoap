using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIBehaviour), true)]
public class AIBehaviourEditor : Editor
{
   private SerializedProperty _showComponentsProp;
   private SerializedProperty _allAIModulesProp;
   private bool _prevShowComponents;

   protected virtual void OnEnable()
   {
      _showComponentsProp = serializedObject.FindProperty("_showComponents");
      _allAIModulesProp = serializedObject.FindProperty("_allAIModules");
      _prevShowComponents = _showComponentsProp.boolValue;
   }

   public override void OnInspectorGUI()
   {
      EditorGUI.BeginChangeCheck();
      DrawDefaultInspector();
      serializedObject.Update();

      if (EditorGUI.EndChangeCheck())
      {
         if (_showComponentsProp.boolValue != _prevShowComponents)
         {
            for (int i = 0; i < _allAIModulesProp.arraySize; i++)
            {
               AIModule prop = (AIModule)_allAIModulesProp.GetArrayElementAtIndex(i).objectReferenceValue;
               if (_showComponentsProp.boolValue)
               {
                  prop.hideFlags = HideFlags.None;
               }
               else
               {
                  prop.hideFlags = HideFlags.HideInInspector;
               }
            }
         }
         _prevShowComponents = _showComponentsProp.boolValue;
         EditorUtility.SetDirty(target);
      }

      serializedObject.ApplyModifiedProperties();
   }

}
