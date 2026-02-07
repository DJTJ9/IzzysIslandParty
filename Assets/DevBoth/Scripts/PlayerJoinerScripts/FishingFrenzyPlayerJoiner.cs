using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player.Multiplayer
{
    public class FishingFrenzyPlayerJoiner : MonoBehaviour
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        public SO_PlayerCollection playerCollectionFF;

        private int playerIndex;
        //private int npcIndex;

        private void Start()
        {
            playerIndex = 0;
            //npcIndex = 0;
            currentPlayers.Players.Clear();
        }

        public void PlayerJoined(PlayerInput _playerInput)
        {
            currentPlayers.Players.Add(playerCollectionFF.Players[playerIndex]);
            
            _playerInput.gameObject.transform.position = playerCollectionFF.Players[playerIndex].SpawnPoint;
            ++playerIndex;
        }
    }
}