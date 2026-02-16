using FishingGame.NPCs;
using HelperScripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.Player.Multiplayer
{
    public class FishingFrenzyPlayerJoiner : MonoBehaviour
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        [SerializeField] private SO_PlayerCollection npcCollection;
        public SO_PlayerCollection playerCollectionFF;

        private int playerIndex;
        private int humanPlayerIndex;
        private int npcIndex;

        [SerializeField] private PlayerInputManager playerInputManager;

        private void Start()
        {
            playerIndex = 0;
            npcIndex = 0;
            currentPlayers.Players.Clear();
        }

        private void OnDestroy()
        {
            ClearPlayerReferences();
        }

        public void PlayerJoined(PlayerInput _playerInput)
        {
            if (playerCollectionFF == null || playerCollectionFF.Players.Count < 1)
            {
                Debug.Log("No players could join");
                return;
            }

            var parent = _playerInput.gameObject.transform.parent;

            if (_playerInput.gameObject.TryGetComponent(out FishingGameNPCBehaviour npc))
            {
                if (humanPlayerIndex == 1)
                {
                    Identifier cameraHolder = _playerInput.transform.parent.gameObject.GetComponentInChildren<Identifier>();
                    cameraHolder.gameObject.SetActive(false);
                }

                npc.SetPlayerIndex(playerIndex);
                npc.OnNPCJoined(npcCollection.Players[playerIndex - 1].PlayerScore);

                parent.gameObject.name = npcCollection.Players[npc.GetPlayerIndex() - 1].Name;

                currentPlayers.Players.Add(npcCollection.Players[npc.GetPlayerIndex() - 1]);

                parent.transform.position = npcCollection.Players[playerIndex - 1].SpawnPoint;

                ++npcIndex;
                ++playerIndex;

                return;
            }

            playerCollectionFF.Players[playerIndex].PlayerReference = _playerInput.gameObject;

            currentPlayers.Players.Add(playerCollectionFF.Players[playerIndex]);

            parent.transform.position = playerCollectionFF.Players[playerIndex].SpawnPoint;

            ++playerIndex;
            ++humanPlayerIndex;
        }

        public void JoinNPCs()
        {
            if (playerIndex == 1)
                playerInputManager.splitScreen = false;

            var nPCStartIndex = playerIndex - 1;

            for (var i = nPCStartIndex; i < npcCollection.Players.Count; i++)
            {
                Instantiate(npcCollection.Players[i].PlayerReference, npcCollection.Players[i].SpawnPoint,
                    npcCollection.Players[i].PlayerReference.transform.rotation);
            }
        }

        private void ClearPlayerReferences()
        {
            for (int i = 0; i < currentPlayers.Players.Count; i++)
            {
                if (!currentPlayers.Players[i].IsNPC)
                    currentPlayers.Players[i].PlayerReference = null;
            }
        }
    }
}