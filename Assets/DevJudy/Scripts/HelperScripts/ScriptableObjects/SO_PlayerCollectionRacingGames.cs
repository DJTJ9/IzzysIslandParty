using System.Collections.Generic;
using UnityEngine;

namespace Player.Collections
{
    [CreateAssetMenu(fileName = "PlayerCollection", menuName = "Scriptable Objects/Player/PlayerCollection/Racing Games")]
    public class SO_PlayerCollectionRacingGames  : ScriptableObject
    {
      [SerializeField] public List<SO_PlayerRacingGames> Players = new List<SO_PlayerRacingGames>();
    }
}