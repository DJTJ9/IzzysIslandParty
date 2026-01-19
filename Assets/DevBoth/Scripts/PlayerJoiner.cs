using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoiner : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public SO_BowlingBattlePlayer PlayersSO;

    private List<PlayerInput> players = new List<PlayerInput>();
    
    private PlayerInputManager playerInputManager;
    private int playerIndex = 0;

    private void Awake()
    {
        // playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        SpawnPlayer(0);
        SpawnPlayer(1);
        // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
        // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    }

    public void OnPlayerJoined(PlayerInput _playerInput)
    {
        AddPlayer(_playerInput);
    }

    private void AddPlayer(PlayerInput _playerInput)
    {
        players.Add(_playerInput);
        
       // _playerInput.transform.position = SpawnPoints[playerIndex].transform.position;
       //  playerIndex++;
    }
    
    [Button]
    public void SpawnPlayer(int _playerIndex)
    {
        Instantiate(PlayersSO.Players[_playerIndex].PlayerPrefab, PlayersSO.Players[_playerIndex].SpawnPoint, Quaternion.identity);
    }
}
