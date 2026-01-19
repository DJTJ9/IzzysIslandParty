using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Bowling Battle Player", menuName = "Scriptable Objects/Bowling Battle Player", order = 1)]
public class SO_BowlingBattlePlayer : SerializedScriptableObject
{
    public BowlingBallSO[] Players = new BowlingBallSO[4];
}
