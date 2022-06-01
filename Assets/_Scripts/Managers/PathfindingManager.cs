using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using System;

public class PathfindingManager : MonoBehaviourStatic<PathfindingManager>
{
   private Vector2 _maxNodes;
   private GridGraph _gridGraph;

   private void Awake()
   {
      AstarPath.OnLatePostScan += OnLatePostScan;
   }

   private void Start()
   {
      _gridGraph = AstarPath.active.data.gridGraph;
      _maxNodes = new Vector2(_gridGraph.width, _gridGraph.depth);
   }

   private void OnDestroy()
   {
      AstarPath.OnLatePostScan -= OnLatePostScan;
   }

   //todo: instead of recursively getting nodes, maybe create list of walkable nodes
   public Vector3 GetRandomWalkablePosition(int[] exceptionTags)
   {
      if (exceptionTags == null || exceptionTags.Length < 0)
      {
         return GetRandomWalkablePosition();
      }

      GridNodeBase gridNode = _gridGraph.GetNode(UnityEngine.Random.Range(0,
                                                 (int)_maxNodes.x),
                                                 UnityEngine.Random.Range(0, (int)_maxNodes.y));

      if (gridNode.Walkable && !Array.Exists(exceptionTags, tag => tag == gridNode.Tag))
      {
         return (Vector3)gridNode.position;
      }
      else
      {
         return GetRandomWalkablePosition(exceptionTags);
      }

   }

   public Vector3 GetRandomWalkablePosition()
   {
      GridNodeBase gridNode = _gridGraph.GetNode(UnityEngine.Random.Range(0,
                                                 (int)_maxNodes.x),
                                                 UnityEngine.Random.Range(0, (int)_maxNodes.y));

      if (gridNode.Walkable)
      {
         return (Vector3)gridNode.position;
      }
      else
      {
         return GetRandomWalkablePosition();
      }

   }

   public void OnLatePostScan(AstarPath aStarPath)
   {
      //todo: maintain list of walkable nodes. maybe?
   }

}
