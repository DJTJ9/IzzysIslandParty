using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    
    public SO_PlayerCollection PlayerCollectionSO;
    [SerializeField] private SO_PlayerCollection npcCollectionBB;
    [SerializeField] private SO_PlayerInputs activePlayerInputs;

    [SerializeField] private SO_PlayerInputs playerInputs;
    // private List<PlayerInput> players = new List<PlayerInput>();
    
    private int m_playerIndex = 0;

    private void Awake()
    {
        var foundPlayerInputs = FindObjectsByType<PlayerInput>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        
        foreach (var playerInput in foundPlayerInputs)
        {
            playerInput.transform.position = PlayerCollectionSO.Players[m_playerIndex].SpawnPoint;
            ++m_playerIndex;
            // var player = Instantiate(PlayerCollectionSO.Players[m_playerIndex].PlayerPrefab, PlayerCollectionSO.Players[m_playerIndex].SpawnPoint, Quaternion.identity, playerInput.transform);;
            //
            // PlayerCollectionSO.Players[m_playerIndex].InitializePlayer(playerInput.gameObject, PlayerCollectionSO.Players[m_playerIndex], m_playerIndex);
            // player.gameObject.GetComponentInChildren<PlayerControllerBowlingBattle>().BindAndEnablePlayerInput();
            // // playerInput.camera = player.GetComponentInChildren<Camera>();
            // playerInput.uiInputModule = player.GetComponentInChildren<InputSystemUIInputModule>();
            // ++m_playerIndex;
        }
        
        // JoinNPCs();
        
        
        // foreach (var input in playerInputs.PlayerInputs)
        // {
        //     SpawnPlayer(m_playerIndex);
        //     ++m_playerIndex;
        // }
        //
        // playerInputManager = FindFirstObjectByType<PlayerInputManager>();
        // SpawnPlayer(m_playerIndex);
        // SpawnPlayer(m_playerIndex);
        // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
        // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    }
    
    public void JoinNPCsBB()
    {
        for (var i = activePlayerInputs.PlayerInputs.Count - 1; i < npcCollectionBB.Players.Count; i++)
        {
            Instantiate(npcCollectionBB.Players[i].PlayerReference, npcCollectionBB.Players[i].SpawnPoint, Quaternion.identity);
        }
    }

    // public void PlayerJoined(PlayerInput _playerInput)
    // {
    //     AddPlayer(_playerInput);
    // }
    //
    // private void AddPlayer(PlayerInput _playerInput)
    // {
    //     playerInputs.PlayerInputs.Add(_playerInput);
    //     ++m_playerIndex;
    //     
    //     // _playerInput.transform.position = SpawnPoints[m_playerIndex].transform.position;
    //     //  m_playerIndex++;
    // }
    
    private GameObject SpawnPlayer(int _playerIndex)
    {
        var player = Instantiate(PlayerCollectionSO.Players[_playerIndex].PlayerReference, PlayerCollectionSO.Players[_playerIndex].SpawnPoint, Quaternion.identity);
        return player;
    }
}
