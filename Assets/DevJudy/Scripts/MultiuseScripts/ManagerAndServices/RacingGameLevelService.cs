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
        
        public Dictionary<int, Tuple<GameObject, string>> WinnerPlacementOrder {get; protected set;}
        
        public virtual void OnFinishLineCrossed(GameObject _collidingObj) { }
    }
}
