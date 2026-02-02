using UnityEngine;

namespace Player.Collections
{
    [CreateAssetMenu(fileName = "PlayerCollection", menuName = "Scriptable Objects/Player/PlayerCollection/Racing Games")]
    public class SO_PlayerCollectionRacingGames : ScriptableObject
    {
        [SerializeField] public SO_PlayerRacingGames[] Players = new  SO_PlayerRacingGames[4];
    }
}