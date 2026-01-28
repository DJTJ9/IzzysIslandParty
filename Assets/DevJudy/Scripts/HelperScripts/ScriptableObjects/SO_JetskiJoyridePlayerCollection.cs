using UnityEngine;

namespace JetskiGame.Player
{
    [CreateAssetMenu(fileName = "PlayerCollection", menuName = "Scriptable Objects/JetskiJoyride/PlayerCollection")]
    public class SO_JetskiJoyridePlayerCollection : ScriptableObject
    {
        [SerializeField] public SO_JetskiJoyridePlayer[] Players = new  SO_JetskiJoyridePlayer[4];
    }
}