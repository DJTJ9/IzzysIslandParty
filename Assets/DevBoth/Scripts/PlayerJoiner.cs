using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerJoiner : MonoBehaviour
{
    [FormerlySerializedAs("PlayerCollectionSO")]
    [FoldoutGroup("Bowling Battle")]
    public SO_PlayerCollection playerCollectionBB;
    [SerializeField] private SO_PlayerCollection npcCollectionBB;
    [SerializeField] private SO_PlayerCollection currentPlayers;

    [Header("Player Inputs")]
    [SerializeField] private SO_PlayerInputs playerInputsSO;
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
        m_playerIndex = 0;
        m_npcIndex = 0;
        currentPlayers.Players.Clear();
    }

    public void PlayerJoinedBB(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out NPC_BowlingBattle npc))
        {
            _playerInput.gameObject.transform.position = npcCollectionBB.Players[m_npcIndex].SpawnPoint;
            currentPlayers.Players.Add(npcCollectionBB.Players[m_npcIndex]);
            ++m_npcIndex;
            return;
        }
        
        _playerInput.gameObject.transform.position = playerCollectionBB.Players[m_playerIndex].SpawnPoint;
        playerCollectionBB.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, playerCollectionBB.Players[m_playerIndex], m_playerIndex);
        currentPlayers.Players.Add(playerCollectionBB.Players[m_playerIndex]);
        // AddPlayer(_playerInput);
        ++m_playerIndex;
    }

    public void SpawnPlayers()
    {
        foreach (var player in currentPlayers.Players)
        {
            Instantiate(player.PlayerPrefab, player.SpawnPoint, Quaternion.identity);
        }
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
        playerInputsSO.PlayerInputs.Add(_playerInput);
    }
    
    [Button]
    public void SpawnPlayer(int _playerIndex)
    {
        Instantiate(playerCollectionBB.Players[_playerIndex].PlayerPrefab, playerCollectionBB.Players[_playerIndex].SpawnPoint, Quaternion.identity);
    }
}
