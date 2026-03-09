using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowlingBattlePlayerJoiner : MonoBehaviour
{
    [SerializeField] private SO_PlayerCollection currentPlayers;
    
    [FoldoutGroup("Bowling Battle", expanded: true)]
    [SerializeField] private SO_PlayerCollection playerCollectionBB;
    [SerializeField] private SO_PlayerCollection npcCollectionBB;

    private int m_playerIndex = 0;
    private int m_npcIndex = 0;

    /// <summary>
    /// Initializes the player and NPC indices to 0 and clears the list of current players.
    /// </summary>
    private void Start()
    {
        m_playerIndex = 0;
        m_npcIndex = 0;
        currentPlayers.Players.Clear();
    }

    /// <summary>
    /// Adds a new player or NPC to the Bowling Battle game.
    /// Assigns the respective spawn position and configuration for the player or NPC
    /// and updates the current players list.
    /// </summary>
    /// <param name="_playerInput">The `PlayerInput` instance of the joining player or NPC.</param>
    public void PlayerJoinedBB(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out NPC_BowlingBattleController npc))
        {
            _playerInput.gameObject.transform.position = npcCollectionBB.Players[m_npcIndex].SpawnPoint;
            currentPlayers.Players.Add(npcCollectionBB.Players[m_npcIndex]);
            ++m_npcIndex;
            return;
        }

        _playerInput.gameObject.transform.position = playerCollectionBB.Players[m_playerIndex].SpawnPoint;
        playerCollectionBB.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, playerCollectionBB.Players[m_playerIndex], m_playerIndex);
        currentPlayers.Players.Add(playerCollectionBB.Players[m_playerIndex]);
        ++m_playerIndex;
    }
    
    /// <summary>
    /// Instantiates and adds all remaining NPCs to the Bowling Battle game
    /// by spawning them at their predefined spawn points.
    /// </summary>
    public void JoinNPCsBB()
    {
        for (var i = m_playerIndex - 1; i < npcCollectionBB.Players.Count; i++)
        {
            Instantiate(npcCollectionBB.Players[i].PlayerReference, npcCollectionBB.Players[i].SpawnPoint, Quaternion.identity);
        }
    }
}
