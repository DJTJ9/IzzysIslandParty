using UnityEngine;

namespace Pathfinding
{
    public class PathfindingNode : IHeapItem<PathfindingNode>
    {
        public int GCost;
        public int HCost;

        // FCost is the node's total cost
        private int FCost => GCost + HCost;
      
        public readonly int GridPositionX;
        public readonly int GridPositionY;
        public readonly Vector3 WorldPosition;
      
        public readonly bool Walkable;

        public PathfindingNode Parent;
      
        public int HeapIndex { get; set; }
      

        public PathfindingNode(Vector3 _pos, bool _walkable, Vector2 _gridPosition)
        {
            WorldPosition = _pos;
            Walkable = _walkable;
            GridPositionX = (int)_gridPosition.x;
            GridPositionY = (int)_gridPosition.y;
        }
      
        public int CompareTo(PathfindingNode _pathfindingNodeToCompare)
        {
            int compare = FCost.CompareTo(_pathfindingNodeToCompare.FCost);

            if (compare == 0)
                compare = HCost.CompareTo(_pathfindingNodeToCompare.HCost);
            // Return negative compare bc compare returns 1 for the higher number, but in this case it should return 1 if it's lower (since the lower cost has a higher priority)
            return -compare;
        }
    }
}