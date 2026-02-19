using System.Collections;
using FullscreenEditor;
using HelperScripts;
using MultiuseScripts;
using Player.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JetskiGame.Player.Multiplayer
{
    public class RacingGamePlayerJoiner : MonoBehaviour
    {
        [SerializeField] private RacingGameLevelService levelService;
        [SerializeField] private SO_PlayerCollectionRacingGames currentPlayers;
        [SerializeField] private SO_PlayerCollectionRacingGames playerCollection;
        [SerializeField] private SO_PlayerCollectionRacingGames npcCollection;
        [SerializeField] private UnityEvent onLevelLoaded;

        private int playerIndex;
        private int humanPlayerIndex;
        private int npcIndex;

        [Header("NPC joining: ")]
        private bool splitScreen;

        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private Controller npcBehaviour;
        private Controller npcBehaviourInstance;
        
        private void Start()
        {
            onLevelLoaded.Invoke();
            playerIndex = 0;
            currentPlayers.Players.Clear();
        }

        private void OnEnable()
        {
            onLevelLoaded.Invoke();

            splitScreen = playerInputManager.splitScreen;
        }

        private void OnDestroy()
        {
            ClearPlayerReferences();
        }

        public void PlayerJoined(PlayerInput _playerInput)
        {
            playerCollection.Players[playerIndex].PlayerReference = _playerInput.gameObject;

            if (_playerInput.gameObject.TryGetComponent(out npcBehaviourInstance) && npcBehaviourInstance.IsOfType(npcBehaviour.GetType()))
            {
                if (humanPlayerIndex == 1 && levelService is JetskiGameLevelService)
                {
                    Identifier cameraHolder = _playerInput.transform.gameObject.GetComponentInChildren<Identifier>();
                    cameraHolder?.gameObject.SetActive(false);
                }

                npcBehaviourInstance.SetPlayerIndex(playerIndex);

                _playerInput.gameObject.name = npcCollection.Players[npcBehaviourInstance.GetPlayerIndex() - 1].Name;

                currentPlayers.Players.Add(npcCollection.Players[npcBehaviourInstance.GetPlayerIndex() - 1]);
                levelService.OnNPCJoined(_playerInput.gameObject);

                ++npcIndex;
                ++playerIndex;

                return;
            }
            else
            {
               if( _playerInput.gameObject.TryGetComponent(out JetskiController playerController))
                   playerController.OnPlayerJoined(playerCollection.Players[playerIndex]);
            }

            currentPlayers.Players.Add(playerCollection.Players[playerIndex]);

            _playerInput.gameObject.name = playerCollection.Players[playerIndex].Name;
            _playerInput.gameObject.transform.position = playerCollection.Players[playerIndex].SpawnPoint;
            levelService.OnPlayerJoined(_playerInput.gameObject);

            ++humanPlayerIndex;
            ++playerIndex;
            
            var currentPlayerIndex = playerIndex;
            StartCoroutine(WaitForPlayerJoin(_playerInput.gameObject, currentPlayerIndex));
        }

        private IEnumerator WaitForPlayerJoin(GameObject _player, int _currentPlayerIndex)
        {
            yield return new WaitForSeconds(0.5f);

            _player.transform.position = playerCollection.Players[_currentPlayerIndex - 1].SpawnPoint;

            yield return null;
        }

        public void JoinNPCs()
        {
            if (humanPlayerIndex == 1)
                playerInputManager.splitScreen = false;
            else
                playerInputManager.splitScreen = splitScreen;

            
            var nPCStartIndex = playerIndex - 1;

            for (var i = nPCStartIndex; i < npcCollection.Players.Count; i++)
            {
                Instantiate(npcCollection.Players[i].PlayerReference, npcCollection.Players[i].SpawnPoint,
                    npcCollection.Players[i].PlayerReference.transform.rotation);
            }
        }

        public void ClearPlayerReferences()
        {
            for (int i = 0; i < currentPlayers.Players.Count; i++)
            {
                if (!currentPlayers.Players[i].IsNPC)
                    currentPlayers.Players[i].PlayerReference = null;
            }
        }
    }
}