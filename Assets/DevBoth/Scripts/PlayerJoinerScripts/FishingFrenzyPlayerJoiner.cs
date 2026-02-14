using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FishingGame.Player.Multiplayer
{
    public class FishingFrenzyPlayerJoiner : MonoBehaviour
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        public SO_PlayerCollection playerCollectionFF;
        [SerializeField] private UnityEvent onLevelLoaded;

        private int playerIndex;

        private void Start()
        {
            onLevelLoaded.Invoke();
            playerIndex = 0;
            currentPlayers.Players.Clear();
        }

        private void OnEnable()
        {
            onLevelLoaded.Invoke();
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