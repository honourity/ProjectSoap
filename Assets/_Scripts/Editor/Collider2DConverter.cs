using UnityEngine;
using UnityEditor;

[AddComponentMenu("")]
public class Collider2DConverter : MonoBehaviour
{
   [MenuItem("CONTEXT/CompositeCollider2D/Convert To PolygonCollider2D")]
   private static void ToPolygon()
   {
      GameObject gameObject = Selection.activeGameObject;
      CompositeCollider2D composite = gameObject.GetComponent<CompositeCollider2D>();
      PolygonCollider2D polygon = gameObject.AddComponent<PolygonCollider2D>();
      polygon.pathCount = composite.pathCount;

      if (gameObject != null)
      {
         for (int i = 0; i < composite.pathCount; i++)
         {
            Vector2[] currentPath = new Vector2[composite.GetPathPointCount(i)];
            composite.GetPath(i, currentPath);
            polygon.SetPath(i, currentPath);
         }
      }

   }

   [MenuItem("CONTEXT/PolygonCollider2D/Refresh Using Composite")]
   private static void RefreshComposite()
   {
      GameObject gameObject = Selection.activeGameObject;


      if (gameObject != null)
      {
         PolygonCollider2D polygon = gameObject.GetComponent<PolygonCollider2D>();
         CompositeColliderChildrenController cccc = gameObject.GetComponent<CompositeColliderChildrenController>() ??
                                                    gameObject.AddComponent<CompositeColliderChildrenController>();

         cccc.EnableUsingComposite();
         CompositeCollider2D composite = gameObject.AddComponent<CompositeCollider2D>();
         polygon.pathCount = composite.pathCount;

         if (gameObject != null)
         {
            for (int i = 0; i < composite.pathCount; i++)
            {
               Vector2[] currentPath = new Vector2[composite.GetPathPointCount(i)];
               composite.GetPath(i, currentPath);
               polygon.SetPath(i, currentPath);
            }
         }

         DestroyImmediate(composite);
      }


   }
}
