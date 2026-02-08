using UnityEngine;
using UnityEngine.InputSystem;

namespace JetskiGame.Player.Multiplayer
{
    public class RacingGamePlayerJoiner : MonoBehaviour
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        public SO_PlayerCollection PlayerCollection;

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
            currentPlayers.Players.Add(PlayerCollection.Players[playerIndex]);
            
            _playerInput.gameObject.transform.position = PlayerCollection.Players[playerIndex].SpawnPoint;
            ++playerIndex;
        }
    }
}
