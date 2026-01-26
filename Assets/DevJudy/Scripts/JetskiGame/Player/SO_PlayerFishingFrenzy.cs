using UnityEngine;

namespace JetskiGame.Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/JetskiJoyride/Player")]
    public class SO_PlayerFishingFrenzy : ScriptableObject
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private int placement;
        [SerializeField] private string time;
    }
}