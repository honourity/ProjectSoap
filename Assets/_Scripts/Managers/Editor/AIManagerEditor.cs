using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//[CustomEditor(typeof(AIManager))]
public class AIManagerEditor : Editor
{
   private AIManager _aiManager;
   private string[] _aiTypeNames;
   private SerializedProperty _m_scriptProp;
   private SerializedProperty _aiInstancesProp;

   private readonly int _layoutWidth = 105;

   private void OnEnable()
   {
      _aiManager = (AIManager)target;
      _aiTypeNames = System.Enum.GetNames(typeof(Enums.AIType));

      _m_scriptProp = serializedObject.FindProperty("m_Script");
      _aiInstancesProp = serializedObject.FindProperty("AIInstances");

      //Debug.Log("AITypeSettings Count is " + _aiManager.AITypeSettings.Count);
      //Debug.Log("There are " + _aiTypeNames.Length + " ai Types");

      if (_aiManager.AITypeSettings == null)
      {
         //Debug.Log("Creating new list");
         _aiManager.AITypeSettings = new List<AITypePrefabLimitSettings>();
      }


      if (_aiManager.AITypeSettings.Count < _aiTypeNames.Length)
      {
         List<AITypePrefabLimitSettings> tempBindings = new List<AITypePrefabLimitSettings>();

         //Copy the existing bindings
         int i = 0;

         for (i = 0; i < _aiManager.AITypeSettings.Count; i++)
         {
            tempBindings.Add(_aiManager.AITypeSettings[i]);
         }

         //Create the rest of the settings
         for (int j = i; j < _aiTypeNames.Length; j++)
         {
            AITypePrefabLimitSettings tempBinding = new AITypePrefabLimitSettings();
            tempBindings.Add(tempBinding);
         }

         _aiManager.AITypeSettings = tempBindings;
      }
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
      EditorGUILayout.Separator();
      serializedObject.Update();

      EditorGUI.BeginChangeCheck();

      EditorGUI.BeginDisabledGroup(true);
      EditorGUILayout.PropertyField(_m_scriptProp);
      EditorGUI.EndDisabledGroup();

      //DRAW HEADINGS IN BOLD
      EditorGUILayout.BeginHorizontal();
      {
         EditorGUILayout.LabelField("AI Type", EditorStyles.boldLabel, GUILayout.Width(_layoutWidth));
         EditorGUILayout.LabelField("Prefab", EditorStyles.boldLabel, GUILayout.Width(_layoutWidth * 2.2f));
         EditorGUILayout.LabelField("Limit", EditorStyles.boldLabel);
      }
      EditorGUILayout.EndHorizontal();

      //DRAW EACH AI TYPE SETTING
      for (int i = 0; i < _aiTypeNames.Length; i++)
      {
         EditorGUILayout.BeginHorizontal();
         {
            EditorGUILayout.LabelField(_aiTypeNames[i], GUILayout.Width(105));
            _aiManager.AITypeSettings[i].Type = (Enums.AIType)i;
            _aiManager.AITypeSettings[i].Prefab = (GameObject)EditorGUILayout.ObjectField(_aiManager.AITypeSettings[i].Prefab, typeof(GameObject), false, GUILayout.Width(_layoutWidth * 2.2f));
            _aiManager.AITypeSettings[i].Limit = EditorGUILayout.IntField(_aiManager.AITypeSettings[i].Limit);
         }
         EditorGUILayout.EndHorizontal();
      }

      //DRAW AI INSTANCES
      EditorGUILayout.PropertyField(_aiInstancesProp, true);

      if (EditorGUI.EndChangeCheck())
      {
      }

      serializedObject.ApplyModifiedProperties();
   }
}
