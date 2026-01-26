using UnityEngine;

namespace JetskiGame.Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/JetskiJoyride/Player")]
    public class SO_JetskiJoyridePlayer : ScriptableObject
    {
        [SerializeField] public GameObject PlayerPrefab;
        [SerializeField] public Vector3 SpawnPosition;
        [SerializeField] public int Placement;
        [SerializeField] public string Time;
    }
}
