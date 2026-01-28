using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerJoiner : MonoBehaviour
{
    public Transform[] SpawnPoints;
    [FormerlySerializedAs("soPlayers")] public SO_PlayerCollection soPlayerCollection;

    [SerializeField] private SO_PlayerInputs playerInputs;
    // private List<PlayerInput> players = new List<PlayerInput>();
    
    private PlayerInputManager playerInputManager;
    private int m_playerIndex = 0;

    // private void Awake()
    // {
    //     // playerInputManager = FindFirstObjectByType<PlayerInputManager>();
    //     // playerInputManager.onPlayerJoined += PlayerJoined;
    //     SpawnPlayer(0);
    //     // SpawnPlayer(1);
    //     // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
    //     // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    // }
    //
    // private void Start()
    // {
    //     SpawnPlayer(1);
    //     SpawnPlayer(2);
    //     SpawnPlayer(3);
    // }

    public void PlayerJoined(PlayerInput _playerInput)
    {
        _playerInput.gameObject.transform.position = soPlayerCollection.Players[m_playerIndex].SpawnPoint;
        soPlayerCollection.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, soPlayerCollection.Players[m_playerIndex], m_playerIndex);
        ++m_playerIndex;
        // AddPlayer(_playerInput);
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
        Instantiate(soPlayerCollection.Players[_playerIndex].PlayerPrefab, soPlayerCollection.Players[_playerIndex].SpawnPoint, Quaternion.identity);
    }
}
