using Player.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

namespace JetskiJoyride.Player.Multiplayer
{
    public class JetskiJoyridePlayerSpawner : MonoBehaviour
    {
        public SO_PlayerCollectionRacingGames playerCollection;

        private PlayerInputManager playerInputManager;
        private int playerIndex = 0;

        private void Awake()
        {
            SpawnPlayer(0);
        }

        private void Start()
        {
            SpawnPlayer(1);
        }

        public void PlayerJoined(PlayerInput _playerInput)
        {
            _playerInput.gameObject.transform.position = playerCollection.Players[playerIndex].SpawnPoint;
            ++playerIndex;
        }
        
        [Button]
        public void SpawnPlayer(int _playerIndex)
        {
            var player = Instantiate(playerCollection.Players[_playerIndex].PlayerPrefab,
                playerCollection.Players[_playerIndex].SpawnPoint, Quaternion.identity);

            if (player.TryGetComponent(out PlayerInput input))
                input.enabled = true;
        }
    }
}