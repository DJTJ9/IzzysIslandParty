using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    [DefaultExecutionOrder(-100)]
    public class PathGrid : MonoBehaviour
    {
        [SerializeField] private LayerMask untreadableLayer;
        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeSize;

        [SerializeField] private bool showGizmos = false;

        private float nodeDiameter;
        private int gridSizeX, gridSizeY;

        private PathfindingNode[,] grid;

        public int MaxSize
        {
            get { return gridSizeX * gridSizeY; }
        }

        private void Start()
        {
            // Otherwise the grid to worldPosition calculation is messed up
            transform.position = Vector3.zero;

            if (gridWorldSize.x <= 0)
                gridWorldSize.x = 1;

            if (gridWorldSize.x <= 0)
                gridWorldSize.y = 1;

            nodeDiameter = nodeSize * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            CreateGrid();
        }

        private void CreateGrid()
        {
            bool pointIsWalkable = false;
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            grid = new PathfindingNode[gridSizeX, gridSizeY];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeSize) +
                                         Vector3.forward * (y * nodeDiameter + nodeSize);


                    pointIsWalkable = Physics.CheckSphere(worldPoint, nodeDiameter);
                    //pointIsWalkable = !Physics.CheckSphere(worldPoint, nodeDiameter, untreadableLayer);


                    grid[x, y] = new PathfindingNode(worldPoint, pointIsWalkable, new Vector2(x, y));
                }
            }
        }

        public PathfindingNode GetNodeFromWorldPosition(Vector3 _worldPos)
        {
            float percentX = Mathf.Clamp01((_worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
            float percentY = Mathf.Clamp01((_worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            PathfindingNode pathfindingNode = grid[x, y];

            return pathfindingNode;
        }

        public List<PathfindingNode> GetNeighbours(PathfindingNode _pathfindingNode)
        {
            List<PathfindingNode> neighbours = new List<PathfindingNode>();

            // Loop through the neighbours on the x and y
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // The node at x = 0 and y = 0 is the node whose neighbours are being checked
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = _pathfindingNode.GridPositionX + x;
                    int checkY = _pathfindingNode.GridPositionY + y;

                    // Check if the neighbour is in scope
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                        neighbours.Add(grid[checkX, checkY]);
                }
            }

            return neighbours;
        }

        public void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid != null)
            {
                foreach (PathfindingNode node in grid)
                {
                    Gizmos.color = node.Walkable ? Color.white : Color.red;

                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter));
                }
            }
        }
    }
}
