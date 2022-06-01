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
      var _allTilemapColliders = FindObjectsOfType<TilemapCollider2D>();

      //TURN OFF USING COMPOSITE COLLIDER
      for (var i = 0; i < _allTilemapColliders.Length; i++)
      {
         _allTilemapColliders[i].usedByComposite = false;
      }

      //CALL SCAN ON PATH
      _path.Scan();

      //TURN USING COMPOSITE COLLIDER BACK ON

      for (var i = 0; i < _allTilemapColliders.Length; i++)
      {
         _allTilemapColliders[i].usedByComposite = true;
      }
   }

   private void OnPathChanged()
   {
      DoScan();
   }
}
