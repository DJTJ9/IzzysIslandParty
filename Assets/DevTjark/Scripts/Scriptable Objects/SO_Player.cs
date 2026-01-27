using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player", order = 1)]
public class SO_Player : SerializedScriptableObject
{
    public string Name;
    public GameObject PlayerPrefab;
    public GameScoreSO PlayerScore;
    public Vector3 SpawnPoint;
    public bool IsNPC;
}
