using Player.Collections;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigolfMayhemPlayerJoiner : MonoBehaviour
{
    [FoldoutGroup("Goal Camera", expanded: false)]
    [SerializeField] private CinemachineCamera goalCamera;
    
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayers;
    
    [FoldoutGroup("Minigolf Mayhem", expanded: true)]
    [SerializeField] private SO_PlayerCollectionRacingGames playerCollectionMM;
    [SerializeField] private SO_PlayerCollectionRacingGames npcCollectionMM;

    private int m_playerIndex = 0;

    private void Start()
    {
        ResetPlayerData();
    }

    /// <summary>
    /// Handles the logic for adding a new player or NPC to the Minigolf Mayhem game.
    /// Assigns the correct spawn position and configuration based on the player's type,
    /// and updates the list of current players.
    /// </summary>
    /// <param name="_playerInput">The `PlayerInput` instance representing the joining player or NPC.</param>
    public void PlayerJoinedMM(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out GoapRigidbodyMovement _npc))
        {
            _npc.SetPlayerIndex(m_playerIndex);
            currentPlayers.Players.Add(npcCollectionMM.Players[_npc.GetPlayerIndex() - 1]);
            _playerInput.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            ++m_playerIndex;
            return;
        }

        if (_playerInput.gameObject.TryGetComponent(out PlayerControllerMinigolfMayhem _controller))
        {
            _controller.SetPlayerIndex(m_playerIndex);
            _playerInput.transform.parent.GetComponentInChildren<CinemachineInputAxisController>().PlayerIndex = m_playerIndex;
        }

        _playerInput.gameObject.transform.position = playerCollectionMM.Players[m_playerIndex].SpawnPoint;
        
        currentPlayers.Players.Add(playerCollectionMM.Players[m_playerIndex]);
        ++m_playerIndex;

        if (_playerInput.gameObject.TryGetComponent(out TransformRegistration transformRegistration))
        {
            transformRegistration.SetAndRegisterPlayerLocationKey($"Player{m_playerIndex}");
        }
    }

    /// <summary>
    /// Instantiates and adds all remaining NPCs to the current Minigolf Mayhem session,
    /// spawning them at their corresponding predefined spawn points.
    /// </summary>
    public void JoinNPCsMM()
    {
        var nPCStartIndex = m_playerIndex - 1;
        for (var i = nPCStartIndex; i < npcCollectionMM.Players.Count; i++)
        {
            Instantiate(npcCollectionMM.Players[i].PlayerReference, npcCollectionMM.Players[i].SpawnPoint, Quaternion.identity);
        }
    }
    
    /// <summary>
    /// Resets the internal player index and clears the current players list, 
    /// ensuring a clean state before starting a new game or session.
    /// </summary>
    private void ResetPlayerData()
    {
        m_playerIndex = 0;
        currentPlayers.Players.Clear();
    }
}
