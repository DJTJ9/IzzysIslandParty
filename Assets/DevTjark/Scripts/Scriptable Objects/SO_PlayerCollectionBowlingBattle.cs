using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Bowling Battle Player Collection", menuName = "Scriptable Objects/Bowling Battle/Player Collection", order = 1)]
public class SO_PlayerCollectionBowlingBattle : SerializedScriptableObject
{
    public SO_PlayerBowlingBattle[] Players = new SO_PlayerBowlingBattle[4];
}
