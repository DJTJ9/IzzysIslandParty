using System.Collections;
using HurdleGame;
using HurdleGame.LevelService;
using Player.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JetskiGame.Player.Multiplayer
{
    public class RacingGamePlayerJoiner : MonoBehaviour
    {
        [SerializeField] private HurdleGameLevelService levelService;
        [SerializeField] private SO_PlayerCollectionRacingGames currentPlayers;
        [SerializeField] private SO_PlayerCollectionRacingGames playerCollection;
        [SerializeField] private SO_PlayerCollectionRacingGames npcCollection;
        private int playerIndex;
        private int npcIndex;

        private void Start()
        {
            playerIndex = 0;
            npcIndex = 0;
            currentPlayers.Players.Clear();
        }

        public void PlayerJoined(PlayerInput _playerInput)
        {
            playerCollection.Players[playerIndex].PlayerReference = _playerInput.gameObject;
            
            if (_playerInput.gameObject.TryGetComponent(out HurdleGameNPCBehaviour npc))
            {
                npc.SetPlayerIndex(playerIndex);
                
                _playerInput.gameObject.name = npcCollection.Players[npc.GetPlayerIndex() - 1].Name;
                
                currentPlayers.Players.Add(npcCollection.Players[npc.GetPlayerIndex() - 1]);
                levelService.OnNPCJoined(_playerInput.gameObject);
                
                ++playerIndex;
                ++npcIndex;

                return;
            }

            currentPlayers.Players.Add(playerCollection.Players[playerIndex]);

            _playerInput.gameObject.name = playerCollection.Players[playerIndex].Name;
            _playerInput.gameObject.transform.position = playerCollection.Players[playerIndex].SpawnPoint;
            levelService.OnPlayerJoined(_playerInput.gameObject);

            ++playerIndex;
            StartCoroutine(WaitForPlayerJoin(_playerInput.gameObject));
        }

        private IEnumerator WaitForPlayerJoin(GameObject _player)
        {
            yield return new WaitForSeconds(0.5f);

            _player.transform.position = playerCollection.Players[playerIndex - 1].SpawnPoint;

            yield return null;
        }

        public void JoinNPCs()
        {
            var nPCStartIndex = playerIndex - 1;

            for (var i = nPCStartIndex; i < npcCollection.Players.Count; i++)
            {
                Instantiate(npcCollection.Players[i].PlayerReference, npcCollection.Players[i].SpawnPoint, 
                    npcCollection.Players[i].PlayerReference.transform.rotation);
            }
        }
    }
}