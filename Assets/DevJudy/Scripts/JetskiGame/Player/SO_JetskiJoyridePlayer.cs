using UnityEngine;

namespace JetskiGame.Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/JetskiJoyride/Player")]
    public class SO_JetskiJoyridePlayer : SO_Player
    {
        [SerializeField] public int Placement;
        [SerializeField] public string Time;
    }
}
