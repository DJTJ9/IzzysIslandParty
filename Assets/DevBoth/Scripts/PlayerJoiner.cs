using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerJoiner : MonoBehaviour
{
    [FormerlySerializedAs("playerCollectionSO")]
    [FoldoutGroup("Bowling Battle")]
    public SO_PlayerCollection playerCollectionBB;
    [SerializeField] private SO_PlayerCollection npcCollectionBB;

    [Header("Player Inputs")]
    [SerializeField] private SO_PlayerInputs playerInputs;
    // private List<PlayerInput> players = new List<PlayerInput>();
    
    private int m_playerIndex = 0;
    private int m_npcIndex = 0;

    // private void Awake()
    // {
    //     // playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    //     // playerInputManager.onPlayerJoined += PlayerJoinedBB;
    //     SpawnPlayer(0);
    //     // SpawnPlayer(1);
    //     // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
    //     // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    // }
    //
    private void Start()
    {
        // SpawnPlayer(1);
        // SpawnPlayer(2);
        // SpawnPlayer(3);
    }

    public void PlayerJoinedBB(PlayerInput _playerInput)
    {
        // _playerInput.gameObject.GetComponent<PlayerControllerBowlingBattle>().BindPlayerSO(playerCollectionBB.Players[m_playerIndex]);

        if (_playerInput.gameObject.TryGetComponent(out NPC_BowlingBattle npc))
        {
            _playerInput.gameObject.transform.position = npcCollectionBB.Players[m_npcIndex].SpawnPoint;
            ++m_npcIndex;
            return;
        }
        
        _playerInput.gameObject.transform.position = playerCollectionBB.Players[m_playerIndex].SpawnPoint;
        playerCollectionBB.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, playerCollectionBB.Players[m_playerIndex], m_playerIndex);
        ++m_playerIndex;
        // AddPlayer(_playerInput);
    }

    public void JoinNPCsBB()
    {
        for (var i = m_playerIndex - 1; i < npcCollectionBB.Players.Count; i++)
        {
            Instantiate(npcCollectionBB.Players[i].PlayerPrefab, npcCollectionBB.Players[i].SpawnPoint, Quaternion.identity);
        }
    }

    public void PlayerJoinedJJ(PlayerInput _playerInput)
    {
        _playerInput.gameObject.transform.position = playerCollectionBB.Players[m_playerIndex].SpawnPoint;
        ++m_playerIndex;
    }
    
    private void AddPlayer(PlayerInput _playerInput)
    {
        playerInputs.PlayerInputs.Add(_playerInput);
        ++m_playerIndex;
        
       // _playerInput.transform.position = SpawnPoints[m_playerIndex].transform.position;
       //  m_playerIndex++;
    }
    
    [Button]
    public void SpawnPlayer(int _playerIndex)
    {
        Instantiate(playerCollectionBB.Players[_playerIndex].PlayerPrefab, playerCollectionBB.Players[_playerIndex].SpawnPoint, Quaternion.identity);
    }
}
