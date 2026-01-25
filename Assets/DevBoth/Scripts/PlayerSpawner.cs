using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] SpawnPoints;
    [FormerlySerializedAs("soPlayers")] [FormerlySerializedAs("PlayersSO")] public SO_PlayerCollectionBowlingBattle soPlayerCollection;

    [SerializeField] private SO_PlayerInputs playerInputs;
    // private List<PlayerInput> players = new List<PlayerInput>();
    
    private PlayerInputManager playerInputManager;
    private int playerIndex = 0;

    private void Awake()
    {
        foreach (var input in playerInputs.PlayerInputs)
        {
            SpawnPlayer(playerIndex);
            ++playerIndex;
        }
        
        // playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        // SpawnPlayer(playerIndex);
        // SpawnPlayer(playerIndex);
        // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
        // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    }

    // public void PlayerJoined(PlayerInput _playerInput)
    // {
    //     AddPlayer(_playerInput);
    // }
    //
    // private void AddPlayer(PlayerInput _playerInput)
    // {
    //     playerInputs.PlayerInputs.Add(_playerInput);
    //     ++playerIndex;
    //     
    //     // _playerInput.transform.position = SpawnPoints[playerIndex].transform.position;
    //     //  playerIndex++;
    // }
    
    [Button]
    public void SpawnPlayer(int _playerIndex)
    {
        Instantiate(soPlayerCollection.Players[_playerIndex].PlayerPrefab, soPlayerCollection.Players[_playerIndex].SpawnPoint, Quaternion.identity);
    }
}
