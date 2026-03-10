using Sirenix.OdinInspector;
using UnityEngine;

namespace MultiuseScripts
{
    public abstract class RacingGameLevelService : LevelServiceParent
    {
        [Header("Placement: ")]
        [SerializeField] protected bool checkPlacements;
        public bool CheckPlacements {get => checkPlacements; protected set => checkPlacements = value;}
        
        [ShowIf("checkPlacements")]
        [SerializeField] protected GameObject[] placementOrder;
        [SerializeField] protected LayerMask playerLayerMask;

        
        [field: SerializeField] public bool IsPVP { get; protected set; }
        
        public virtual void OnPlayerJoined(GameObject _player) { }
        
        public virtual void OnNPCJoined(GameObject _npc) { }
        
        public virtual void OnFinishLineCrossed(GameObject _collidingObj) { }
    }
}
