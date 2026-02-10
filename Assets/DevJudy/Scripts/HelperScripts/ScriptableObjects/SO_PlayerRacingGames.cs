using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player/Racing Player")]
    public class SO_PlayerRacingGames : SO_Player
    {
        [SerializeField] public string Time;
    }
}