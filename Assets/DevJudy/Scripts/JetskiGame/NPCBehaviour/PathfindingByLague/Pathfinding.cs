using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private PathGrid pathGrid;
        private PathRequestManager pathRequestManager;

        private Heap<PathfindingNode> openSet = new Heap<PathfindingNode>(1);
        private HashSet<PathfindingNode> closedSet;

        private void Awake()
        {
            pathGrid = GetComponent<PathGrid>();
            if (pathGrid == null)
                Debug.LogError("Grid not found");

            pathRequestManager = GetComponent<PathRequestManager>();
            if (pathRequestManager == null)
                Debug.LogError("PathRequestManager not found");
        }

        public void StartFindPath(Vector3 _startPos, Vector3 _targetPos)
        {
            StartCoroutine(FindPath(_startPos, _targetPos));
        }

        private IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] pathPoints = new Vector3[0];
            bool pathFound = false;

            PathfindingNode startPathfindingNode = pathGrid.GetNodeFromWorldPosition(_startPos);
            PathfindingNode targetPathfindingNode = pathGrid.GetNodeFromWorldPosition(_targetPos);

            // Maybe invert the if so there are less brackets
            if (startPathfindingNode.Walkable && targetPathfindingNode.Walkable)
            {
                openSet = new Heap<PathfindingNode>(pathGrid.MaxSize);
                closedSet = new HashSet<PathfindingNode>();

                openSet.Add(startPathfindingNode);

                while (openSet.Count > 0)
                {
                    PathfindingNode currentPathfindingNodeToCheck = openSet.RemoveFirst();
                    closedSet.Add(currentPathfindingNodeToCheck);

                    if (currentPathfindingNodeToCheck == targetPathfindingNode)
                    {
                        sw.Stop();
                        pathFound = true;

                        Debug.Log(string.Format("Path found in {0}ms", sw.ElapsedMilliseconds));
                        break;
                    }

                    foreach (PathfindingNode neighbour in pathGrid.GetNeighbours(currentPathfindingNodeToCheck))
                    {
                        if (!neighbour.Walkable || closedSet.Contains(neighbour))
                            continue;

                        int newCostToNeighbour = currentPathfindingNodeToCheck.GCost + GetDistance(currentPathfindingNodeToCheck, neighbour);

                        if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                        {
                            neighbour.GCost = newCostToNeighbour;
                            neighbour.HCost = GetDistance(neighbour, targetPathfindingNode);

                            neighbour.Parent = currentPathfindingNodeToCheck;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                        }
                    }
                }
            }

            yield return null;

            if (pathFound)
                pathPoints = RetracePath(startPathfindingNode, targetPathfindingNode);

            pathRequestManager.FinishedProcessingPath(pathPoints, pathFound);
        }

        private Vector3[] RetracePath(PathfindingNode _startPathfindingNode, PathfindingNode _targetPathfindingNode)
        {
            List<PathfindingNode> path = new List<PathfindingNode>();
            PathfindingNode currentPathfindingNode = _targetPathfindingNode;

            while (currentPathfindingNode.WorldPosition != _startPathfindingNode.WorldPosition)
            {
                path.Add(currentPathfindingNode);
                currentPathfindingNode = currentPathfindingNode.Parent;
            }

            Vector3[] pathPoints = SimplifyPath(path);

            Array.Reverse(pathPoints);

            return pathPoints;
        }

        private Vector3[] SimplifyPath(List<PathfindingNode> _path)
        {
            List<Vector3> pathPoints = new List<Vector3>();
            Vector2 previousDirection = Vector2.zero;
            Vector2 newDirection = Vector2.zero;

            for (int i = 1; i < _path.Count; i++)
            {
                newDirection = new Vector2(_path[i - 1].GridPositionX - _path[i].GridPositionX, _path[i - 1].GridPositionY - _path[i].GridPositionY);

                if (newDirection != previousDirection)
                    pathPoints.Add(_path[i].WorldPosition);

                previousDirection = newDirection;
            }

            return pathPoints.ToArray();
        }

        private int GetDistance(PathfindingNode _a, PathfindingNode _b)
        {
            int distanceX = Mathf.Abs(_a.GridPositionX - _b.GridPositionX);
            int distanceY = Mathf.Abs(_a.GridPositionY - _b.GridPositionY);

            // 14 is the cost to go diagonally, 10 is the cost to go straight 
            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
