using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileRenamer : Editor
{

   [MenuItem("Tools/Rename Tiles by Sprite")]
   public static void RenameTilesBySprite()
   {
      Object[] allSelected = Selection.objects;
      
      foreach(object selected in allSelected)
      {
         if (selected is Tile)
         {
            Tile tile = (Tile)selected;
            //Debug.Log();
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tile), tile.sprite.name);
         }
      }
   }
}
