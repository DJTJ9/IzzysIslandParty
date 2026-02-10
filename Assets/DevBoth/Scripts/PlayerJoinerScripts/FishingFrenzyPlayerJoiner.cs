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
            playerCollectionFF.Players[playerIndex].PlayerReference = _playerInput.gameObject;
            
            currentPlayers.Players.Add(playerCollectionFF.Players[playerIndex]);
            var parent = _playerInput.gameObject.transform.parent;
            
            parent.transform.position = playerCollectionFF.Players[playerIndex].SpawnPoint;
            ++playerIndex;
        }
    }
}