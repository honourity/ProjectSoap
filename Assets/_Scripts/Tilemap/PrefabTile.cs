using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Tilemaps
{
   /// <summary>
   /// A Tile that creates a GameObject at runtime.
   /// </summary>
   [Serializable]
   public class PrefabTile : TileBase
   {
      /// <summary>
      /// The GameObject that will be created by the Tile.
      /// </summary>
      [SerializeField] public GameObject Prefab;

      /// <summary>
      /// The Sprite that will be displayed in the Editor.
      /// </summary>
      private Sprite Sprite => Prefab.GetComponent<SpriteRenderer>().sprite;

      /// <summary>
      /// Allows the TileData for this Tile to be modified.
      /// </summary>
      /// <param name="position"></param>
      /// <param name="tilemap"></param>
      /// <param name="tileData"></param>
      public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
      {
         tileData.gameObject = Prefab;
         tileData.sprite = Sprite;
      }

      /// <summary>
      /// Allows the GameObject for this Tile to be modified when the game starts.
      /// </summary>
      /// <param name="position"></param>
      /// <param name="tilemap"></param>
      /// <param name="gameObject"></param>
      /// <returns></returns>
      public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject gameObject)
      {
         gameObject.transform.position += new Vector3(0.5f, 0.5f, 0);

         return base.StartUp(position, tilemap, gameObject);
      }

#if UNITY_EDITOR
      /// <summary>
      /// Registers a menu item that creates a new PrefabTile asset.
      /// </summary>
      [MenuItem("Assets/Create/Prefab Tile")]
      public static void CreatePrefabTile()
      {
         string path = EditorUtility.SaveFilePanelInProject(
             "Save Prefab Tile",
             "New Prefab Tile",
             "asset",
             "Save Prefab Tile",
             "Assets"
         );

         if (path == "")
         {
            return;
         }

         AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PrefabTile>(), path);
      }
#endif
   }
}