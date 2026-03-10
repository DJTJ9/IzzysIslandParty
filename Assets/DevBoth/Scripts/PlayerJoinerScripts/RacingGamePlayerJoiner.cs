using System.Collections;
using System.Collections.Generic;
using CharacterCreator;
using HelperScripts;
using JetskiGame.UI;
using MultiuseScripts;
using Player.Collections;
using Sirenix.OdinInspector;
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
        private CharacterCreatorService characterCreatorService;
        
        [Header("Player layer: ")]
        [SerializeField] private bool setPlayerLayer = false;
        [ShowIf("setPlayerLayer")]
        [SerializeField] private List<int> playerLayerIndex;

        [Header("NPC joining: ")]
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private Controller npcBehaviour;
        private Controller npcBehaviourInstance;
        
        private int playerIndex;
        private int humanPlayerIndex;
        private int npcIndex;
        private bool splitScreen;


        private void Start()
        {
            characterCreatorService = GetComponent<CharacterCreatorService>();

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
            if (playerIndex == 0)
                Time.timeScale = 1;
            
            GameObject playerObj = _playerInput.gameObject;
            
            playerCollection.Players[playerIndex].PlayerReference = playerObj;
        
            if (playerObj.TryGetComponent(out npcBehaviourInstance) && npcBehaviourInstance.GetType() == npcBehaviour.GetType())
            {
                NPCJoined(playerObj, _playerInput);
                return;
            }

            PlayerCharacterJoined(playerObj, _playerInput);
        }


        private void NPCJoined(GameObject _playerObj, PlayerInput _playerInput)
        {
            if (humanPlayerIndex == 1 && levelService is JetskiGameLevelService)
            {
                Identifier cameraHolder = _playerInput.transform.gameObject.GetComponentInChildren<Identifier>();
                cameraHolder?.gameObject.SetActive(false);
            }

            npcBehaviourInstance.SetPlayerIndex(playerIndex);
            npcBehaviourInstance.OnNPCJoined(npcCollection.Players[npcBehaviourInstance.GetPlayerIndex() - 1]);
            
            _playerObj.name = npcCollection.Players[npcBehaviourInstance.GetPlayerIndex() - 1].Name;

            currentPlayers.Players.Add(npcCollection.Players[npcBehaviourInstance.GetPlayerIndex() - 1]);
            levelService.OnNPCJoined(_playerObj);
            
            PlayerMeshIdentifier npcMesh = _playerObj.GetComponentInChildren<PlayerMeshIdentifier>();
            if (npcMesh != null)
            {
                characterCreatorService.SetMeshAndMaterial(npcMesh.MeshRenderer, playerIndex);

                if (npcMesh.HasTwoMeshes)
                    characterCreatorService.SetOtherMaterial(npcMesh.OtherMeshRenderer, playerIndex);
            }

            ++npcIndex;
            ++playerIndex;
        }


        private void PlayerCharacterJoined(GameObject _playerObj, PlayerInput _playerInput)
        {
            if (_playerObj.TryGetComponent(out JetskiController playerController))
                playerController.OnPlayerJoined(playerCollection.Players[playerIndex], playerIndex);

            currentPlayers.Players.Add(playerCollection.Players[playerIndex]);

            _playerObj.name = playerCollection.Players[playerIndex].Name;
            _playerObj.transform.position = playerCollection.Players[playerIndex].SpawnPoint;
            levelService.OnPlayerJoined(_playerInput.gameObject);
            
            PlayerMeshIdentifier playerMesh = _playerObj.GetComponentInChildren<PlayerMeshIdentifier>();
            if (playerMesh != null)
            {
                characterCreatorService.SetMeshAndMaterial(playerMesh.MeshRenderer, playerIndex);

                if (playerMesh.HasTwoMeshes)
                    characterCreatorService.SetOtherMaterial(playerMesh.OtherMeshRenderer, playerIndex);
            }
        
            if (levelService is JetskiGameLevelService jetskiLevelService)
            {
                if (_playerObj.TryGetComponent(out JetskiGameUIManager jetskiUIManager))
                {
                    jetskiUIManager.OnPlayerJoined(levelService.IsPVP);
                }
            }

            if (playerLayerIndex.Count > playerIndex && setPlayerLayer)
                SetLayerAllChildren(_playerObj.transform);

            ++humanPlayerIndex;
            ++playerIndex;

            var currentPlayerIndex = playerIndex;
            StartCoroutine(WaitForPlayerJoin(_playerObj, currentPlayerIndex));
        }

        private void SetLayerAllChildren(Transform _root)
        {
            var children = _root.GetComponentsInChildren<Transform>(includeInactive: true);

            if (children.Length < 1)
                return;

            foreach (var child in children)
            {
                if (!child.gameObject)
                    continue;

                child.gameObject.layer = (playerLayerIndex[playerIndex]);
            }
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