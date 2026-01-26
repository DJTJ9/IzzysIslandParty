using JetskiGame.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

namespace JetskiJoyride.Player.Multiplayer
{
    public class JetskiJoyridePlayerSpawner : MonoBehaviour
    {
        public SO_JetskiJoyridePlayerCollection soJetskiJoyridePlayerCollection;
        //[SerializeField] private SO_PlayerInputs playerInputs;

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
            _playerInput.gameObject.transform.position = soJetskiJoyridePlayerCollection.Players[playerIndex].SpawnPosition;
            ++playerIndex;
        }

        //  private void AddPlayer(PlayerInput _playerInput)
        //  {
        //      //playerInputs.PlayerInputs.Add(_playerInput);
        //      ++playerIndex;
        //  }

        [Button]
        public void SpawnPlayer(int _playerIndex)
        {
            var player = Instantiate(soJetskiJoyridePlayerCollection.Players[_playerIndex].PlayerPrefab,
                soJetskiJoyridePlayerCollection.Players[_playerIndex].SpawnPosition, Quaternion.identity);

            if (player.TryGetComponent(out PlayerInput input))
                input.enabled = true;
        }
    }
}