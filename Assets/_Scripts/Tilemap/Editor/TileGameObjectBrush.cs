using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace UnityEditor
{
   [CustomGridBrush(true, false, false, "Tile Gameobject Brush")]
   public class TileGameObjectBrush : UnityEditor.Tilemaps.GridBrush
   {
      [Header("Paint Settings")]
      [SerializeField] private bool _replaceExistingTiles = true;
      [SerializeField] private bool _createColliders = true;
      [SerializeField] private bool _compositeCollider = true;

      #region PAINT 
      public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
      {
         if (Selection.activeGameObject == null)
         {
            Debug.LogWarning("Tile GameObject Brush requires a parent GameObject to be selected in the Hierarchy");
            return;
         }

         CompositeColliderChildrenController cccc = Selection.activeGameObject.GetComponentInParent<CompositeColliderChildrenController>();
         cccc?.DisableUsingComposite();

         Vector3Int min = position - pivot;
         BoundsInt bounds = new BoundsInt(min, size);
         BoxFill(grid, brushTarget, bounds);
         cccc?.EnableUsingComposite();
      }

      public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
      {
         foreach (Vector3Int pos in position.allPositionsWithin)
         {
            //Check if there's already a gameobject at this position and delete it
            if (_replaceExistingTiles)
            {
               Vector2 overlapPosition = pos + new Vector3(0.5f, 0.5f);
               Collider2D collider = Physics2D.OverlapBox(overlapPosition, Vector2.one * 0.95f, 0f);

               if (collider != null && collider.transform.parent?.gameObject == Selection.activeGameObject)
               {
                  Undo.DestroyObjectImmediate(collider.gameObject);
               }

            }

            Vector3Int local = pos - position.min;
            BrushCell cell = cells[GetCellIndexWrapAround(local.x, local.y, local.z)];
            PaintCell(pos, cell);
         }
      }

      private void PaintCell(Vector3Int position, BrushCell cell)
      {
         Transform newSprite = new GameObject(((Tile)cell.tile).sprite.name, typeof(SpriteRenderer)).transform;

         newSprite.parent = Selection.activeGameObject?.transform;
         newSprite.position = position + new Vector3(0.5f, 0.5f, 0);
         SpriteRenderer spriteRenderer = newSprite.GetComponent<SpriteRenderer>();
         spriteRenderer.sprite = ((Tile)cell.tile).sprite;

         if (_createColliders)
         {
            Collider2D _collider = newSprite.gameObject.AddComponent<PolygonCollider2D>();

            if (_compositeCollider)
            {
               _collider.usedByComposite = true;
            }
         }

         Undo.RegisterCreatedObjectUndo(newSprite.gameObject, "Paint GameObject");
      }


      #endregion

      #region ERASE
      public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
      {
         if (Selection.activeGameObject == null)
         {
            Debug.LogWarning("Tile GameObject Brush requires a parent GameObject to be selected in the Hierarchy");
            return;
         }

         CompositeColliderChildrenController cccc = Selection.activeGameObject.GetComponentInParent<CompositeColliderChildrenController>();
         cccc?.DisableUsingComposite();

         Vector3Int min = position - pivot;
         BoundsInt bounds = new BoundsInt(min, size);
         BoxErase(gridLayout, brushTarget, bounds);
         cccc?.EnableUsingComposite();
      }

      public override void BoxErase(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
      {
         foreach (Vector3Int pos in position.allPositionsWithin)
         {
            Vector2 overlapPosition = pos + new Vector3(0.5f, 0.5f);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(overlapPosition, Vector2.one * 0.9f, 0f);

            foreach (Collider2D collider in colliders)
            {
               if (collider != null && collider.transform.parent?.gameObject == Selection.activeGameObject)
               {
                  Undo.DestroyObjectImmediate(collider.gameObject);
                  break;
               }
            }
         }
      }

      #endregion

      #region SELECT

      public override void Select(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
      {
         List<GameObject> allGameObjectsToSelect = new List<GameObject>();
         GameObject parent = Selection.activeGameObject;
         Debug.Log("parent: " + parent?.name ?? "null");
         foreach (Vector3Int pos in position.allPositionsWithin)
         {
            Debug.Log("Selecting object at " + pos);
            Vector2 overlapPosition = pos + new Vector3(0.5f, 0.5f);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(overlapPosition, Vector2.one * 0.9f, 0f);

            foreach (Collider2D collider in colliders)
            {
               if (collider != null)
               {
                  Debug.Log("Adding " + collider.name);
                  allGameObjectsToSelect.Add(collider.gameObject);
               }
            }
         }

         Selection.objects = allGameObjectsToSelect.ToArray();
      }
      #endregion
   }


   [CustomEditor(typeof(TileGameObjectBrush))]
   public class TileGameObjectBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
   {
      public override GameObject[] validTargets => null;

      public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
      {
         base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);

         string labelString = Selection.activeGameObject?.name ?? "No GameObject Parent selected";
         Vector3 labelPos = position.position;

         GUIStyle style = new GUIStyle(EditorStyles.helpBox);
         style.normal.textColor = Color.red;
         style.fontSize = 15;

         SceneView sceneView = EditorWindow.GetWindow<SceneView>();
         Vector3 mousePosition = Event.current.mousePosition;
         mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y + 32f;
         mousePosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
         Handles.Label(mousePosition, labelString, style);
      }
   }
}