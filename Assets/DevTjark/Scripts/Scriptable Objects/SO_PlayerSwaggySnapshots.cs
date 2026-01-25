using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Swaggy Snapshots", menuName = "Scriptable Objects/Swaggy Snapshots/Player", order = 1)]
public class SO_PlayerSwaggySnapshots : SerializedScriptableObject
{
    public GameObject PlayerPrefab;
    public GameScoreSO PlayerScore;
}
