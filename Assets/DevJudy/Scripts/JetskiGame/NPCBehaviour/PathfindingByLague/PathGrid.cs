using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    [DefaultExecutionOrder(-100)]
    public class PathGrid : MonoBehaviour
    {
        [SerializeField] private TerrainType[] walkableTerrains;
        [SerializeField] private LayerMask untreadableLayer;
        private LayerMask walkableLayer;

        [SerializeField] private int obstacleProximityPenalty = 10;

        [SerializeField] private Vector2 gridWorldSize;
        [SerializeField] private float nodeSize;

        [SerializeField] private bool showGizmos = false;

        private float nodeDiameter;
        private int gridSizeX, gridSizeY;

        private PathfindingNode[,] grid;

        private Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

        private int penaltyMin = int.MaxValue;
        private int penaltyMax = int.MinValue;

        public int MaxSize
        {
            get { return gridSizeX * gridSizeY; }
        }

        private void Awake()
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

            foreach (TerrainType terrain in walkableTerrains)
            {
                walkableLayer.value |= terrain.TerrainMask.value;
                walkableRegionsDictionary.Add((int)Mathf.Log(terrain.TerrainMask.value, 2), terrain.TerrainPenalty);
            }

            CreateGrid();
        }

        private void CreateGrid()
        {
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

            grid = new PathfindingNode[gridSizeX, gridSizeY];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeSize) +
                                         Vector3.forward * (y * nodeDiameter + nodeSize);
                    
                    bool pointIsWalkable = !Physics.CheckSphere(worldPoint, nodeDiameter, untreadableLayer);

                    int movementPenalty = 0;

                    if (pointIsWalkable)
                    {
                        Ray ray = new Ray(worldPoint + Vector3.up * 25, Vector3.down);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 50f, walkableLayer))
                        {
                            walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                        }
                    }
                    else
                        movementPenalty += obstacleProximityPenalty;

                    grid[x, y] = new PathfindingNode(worldPoint, pointIsWalkable, new Vector2(x, y), movementPenalty);
                }
            }

            BlurPenaltyMap(3);
        }

        public PathfindingNode GetNodeFromWorldPosition(Vector3 _worldPos)
        {
            float percentX = Mathf.Clamp01((_worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
            float percentY = Mathf.Clamp01((_worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            return grid[x, y];
        }

        private void BlurPenaltyMap(int _blurSize)
        {
            int kernelSize = _blurSize * 2 + 1;
            int kernelExtents = kernelSize - 1 / 2;

            int[,] penaltyHorizontal = new int[gridSizeX, gridSizeY];
            int[,] penaltyVertical = new int[gridSizeX, gridSizeY];

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = -kernelExtents; x <= kernelExtents; x++)
                {
                    int sampleX = Mathf.Clamp(x, 0, kernelExtents);

                    penaltyHorizontal[0, y] += grid[sampleX, y].MovementPenalty;
                }

                for (int x = 1; x < gridSizeX; x++)
                {
                    int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                    int addIndex = Mathf.Clamp(x + kernelExtents - 1, 0, gridSizeX - 1);

                    penaltyHorizontal[x, y] = penaltyHorizontal[x - 1, y] - grid[removeIndex, y].MovementPenalty + grid[addIndex, y].MovementPenalty;
                }
            }

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = -kernelExtents; y <= kernelExtents; y++)
                {
                    int sampleY = Mathf.Clamp(x, 0, kernelExtents);

                    penaltyVertical[x, 0] += penaltyHorizontal[x, sampleY];
                }

                for (int y = 1; y < gridSizeY; y++)
                {
                    int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                    int addIndex = Mathf.Clamp(y + kernelExtents - 1, 0, gridSizeY - 1);

                    penaltyVertical[x, y] = penaltyVertical[x, y - 1] - penaltyHorizontal[x, removeIndex] + penaltyHorizontal[x, addIndex];

                    int blurredPenalty = Mathf.RoundToInt((float)penaltyVertical[x, y] / (kernelSize * kernelSize));
                    grid[x, y].MovementPenalty = blurredPenalty;

                    if (blurredPenalty > penaltyMax)
                        penaltyMax = blurredPenalty;

                    if (blurredPenalty < penaltyMin)
                        penaltyMin = blurredPenalty;
                }
            }

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
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, node.MovementPenalty));

                    Gizmos.color = node.Walkable ? Gizmos.color : Color.red;

                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (nodeDiameter));
                }
            }
        }
    }
}