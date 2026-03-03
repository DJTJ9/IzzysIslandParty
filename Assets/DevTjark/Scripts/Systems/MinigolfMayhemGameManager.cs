using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfMayhemGameManager : MonoBehaviour
{
    [FoldoutGroup("Game Mode", expanded: true)]
    public bool ClassicMode;
    public bool RacingMode;
}
