using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(InventoryComponent))]
public class InventoryComponentEditor : Editor
{
   private InventoryComponent inventoryComponent;
   private IList<IInventoryItem> _items;

   private void OnEnable()
   {
      inventoryComponent = (InventoryComponent)target;
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
      _items = inventoryComponent.GetItems();
      string inventoryString = "List of Inventory: \n";

      foreach (var item in _items)
      {
         if (item is SoapModel)
         {
            inventoryString += "Soap";
         }
         else if (item is WaterModel)
         {
            WaterModel water = (WaterModel)item;
            inventoryString += "Water: " + water.Volume;
         }

         inventoryString += "\n";
      }

      EditorGUILayout.HelpBox(inventoryString, MessageType.Info);
   }

}
