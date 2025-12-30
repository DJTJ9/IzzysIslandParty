using UnityEngine;

namespace Pathfinding
{
    [System.Serializable]
    public class TerrainType : MonoBehaviour
    {
        public LayerMask TerrainMask;
        public int TerrainPenalty;
    }
}