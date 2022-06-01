using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public static class PathfindingUtility
{
   public static IEnumerator FollowPath(List<Vector3> vectorPath, Transform transform, Rigidbody2D rb2D, MovementController moveController, Enums.MoveType moveType = Enums.MoveType.RUN_LAND, Action<bool> onComplete = null)
   {
      bool breakage = false;

      Run run = null;

      for (int i = 0; i < vectorPath.Count; i++)
      {
         run = null;
         breakage = false;

         if (Vector2.Distance(transform.position, vectorPath[i]) < AstarPath.active.data.gridGraph.nodeSize / 4f)
            continue;

         Vector3 direction = (vectorPath[i] - transform.position).normalized;
         moveController.Move(moveType, direction);
         Vector3 cachedPos = vectorPath[i];

         while (Vector2.Distance(transform.position, cachedPos) > 0.25f && !breakage)
         {
#if UNITY_EDITOR
            DebugExtension.DebugPoint(vectorPath[i], Color.green, 0.25f);
#endif

            //Check if we've run into some wall and stop what we're doing if we keep moving for 1 second without any velocity
            if (rb2D.velocity.magnitude < Mathf.Epsilon)
            {
               if (run == null)
               {
                  run = Run.After(1f, () =>
                  {
                     breakage = true;
                  });
               }
            }
            else
            {
               if (run != null && !run.isDone)
                  run.Abort();
            }

            yield return null;
         }

         if (breakage)
         {
            onComplete(false);
            yield break;
         }
      }

      if (run != null)
      {
         run.Abort();
      }

      if (onComplete != null && !breakage)
      {
         onComplete.Invoke(!breakage);
      }
      yield return null;

   }

   public static IEnumerator StartShortestPath(Component[] components, Transform transform, Seeker seeker, Action<Path> onComplete)
   {
      float shortestDistance = Mathf.Infinity;
      Path shortestPath = null;
      int index = 0;
      for (index = 0; index < components.Length; index++)
      {
         Path tempPath = seeker.StartPath(transform.position, components[index].transform.position);
         yield return CoroutineHelper.Instance.StartCoroutine(tempPath.WaitForPath());

         float pathDistance = tempPath.GetTotalLength();

         if (pathDistance < shortestDistance)
         {
            shortestPath = tempPath;
            shortestDistance = pathDistance;
         }
      }

      onComplete.Invoke(shortestPath);
   }

   //Singular version
   public static int GetTagNumByName(string tagName)
   {
      string[] allTagNames = AstarPath.FindTagNames();
      return Array.IndexOf(allTagNames, tagName);
   }

   //Array version
   public static int[] GetTagNumsByNames(string[] tagNames)
   {
      string[] allTagNames = AstarPath.FindTagNames();
      int[] tagNums = new int[tagNames.Length];

      for (int i = 0; i < tagNums.Length; i++)
      {
         tagNums[i] = Array.IndexOf(allTagNames, tagNames[i]);
      }

      return tagNums;
   }
}
