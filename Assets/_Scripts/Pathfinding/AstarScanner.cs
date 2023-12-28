using UnityEngine;
using UnityEngine.Tilemaps;
using Enums;

public class AstarScanner : MonoBehaviour
{
   private AstarPath _path;

   private void Start()
   {
      Messaging.AddListener(MessageType.PATH_CHANGED, OnPathChanged);
      _path = GetComponent<AstarPath>();
      DoScan();
   }

   private void OnDestroy()
   {
      Messaging.RemoveListener(MessageType.PATH_CHANGED, OnPathChanged);
   }

   public void DoScan()
   {
      if (_path == null)
      {
         Start();
      }

      //GET ALL TILEMAP COLLIDERS
      var _allTilemapColliders = FindObjectsByType<TilemapCollider2D>(FindObjectsSortMode.None);

      //TURN OFF USING COMPOSITE COLLIDER
      for (var i = 0; i < _allTilemapColliders.Length; i++)
      {
         _allTilemapColliders[i].compositeOperation = Collider2D.CompositeOperation.None;
      }

      //CALL SCAN ON PATH
      _path.Scan();

      //TURN USING COMPOSITE COLLIDER BACK ON

      for (var i = 0; i < _allTilemapColliders.Length; i++)
      {
         _allTilemapColliders[i].compositeOperation = Collider2D.CompositeOperation.Merge;
      }
   }

   private void OnPathChanged()
   {
      DoScan();
   }
}
