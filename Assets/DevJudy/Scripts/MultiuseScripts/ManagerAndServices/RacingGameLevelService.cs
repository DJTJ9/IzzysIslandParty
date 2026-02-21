using System;
using System.Collections.Generic;
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
        
        public virtual void OnPlayerJoined(GameObject _player) { }
        
        public virtual void OnNPCJoined(GameObject _npc) { }
        
        public virtual void OnFinishLineCrossed(GameObject _collidingObj) { }
    }
}
