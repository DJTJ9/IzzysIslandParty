using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Bowling Battle Player", menuName = "Scriptable Objects/Bowling Battle Player", order = 1)]
public class SO_PlayersBowlingBattle : SerializedScriptableObject
{
    public SO_PlayerBowlingBattle[] Players = new SO_PlayerBowlingBattle[4];
}
