using UnityEngine;
using UnityEditor;
using System.IO;

public class ToJSON : Editor
{
   [MenuItem("CONTEXT/Object/To JSON")]
   public static void ConvertToJSON(MenuCommand menuCommand)
   {
      string dataAsJson = EditorJsonUtility.ToJson(menuCommand.context, true);
      string filePath = Application.dataPath + "/data.json";
      File.WriteAllText(filePath, dataAsJson);
   }
}
