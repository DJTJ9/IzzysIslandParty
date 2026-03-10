using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SwaggySnapshotsPlayerJoiner : MonoBehaviour
{
    [SerializeField] private SO_SwaggySnapshotsPlayerCollection currentPlayers;
    
    [FoldoutGroup("Scriptable Objects", expanded: true)]
    [SerializeField] private SO_SwaggySnapshotsPlayerCollection playerCollectionSS;
    [SerializeField] private SO_SwaggySnapshotsPlayerCollection npcCollectionSS;
    [SerializeField] private UnityEvent onLevelLoaded;

    private int m_playerIndex = 0;

    /// <summary>
    /// Initializes the player index, clears the current players list,
    /// and invokes the level loaded event to prepare the game state.
    /// </summary>
    private void Start()
    {
        onLevelLoaded.Invoke();
        m_playerIndex = 0;
        currentPlayers.Players.Clear();
    }

    /// <summary>
    /// Adds a player or NPC to the Swaggy Snapshots game, assigning
    /// the correct player data and updating the current players list.
    /// </summary>
    /// <param name="_playerInput">The `PlayerInput` instance of the joining player or NPC.</param>
    public void PlayerJoined(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out NPC_SwaggySnapshots npc))
        {
            npc.SetPlayerIndex(m_playerIndex);
            currentPlayers.Players.Add(npcCollectionSS.Players[npc.GetPlayerIndex() - 1]);
            ++m_playerIndex;
            return;
        }

        playerCollectionSS.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, playerCollectionSS.Players[m_playerIndex], m_playerIndex);
        currentPlayers.Players.Add(playerCollectionSS.Players[m_playerIndex]);
        ++m_playerIndex;
    }
    
    /// <summary>
    /// Instantiates and adds all remaining NPCs to the game by
    /// spawning them at their respective predefined spawn points.
    /// </summary>
    public void JoinNPCs()
    {
        var nPCStartIndex = m_playerIndex - 1;
        for (var i = nPCStartIndex; i < npcCollectionSS.Players.Count; i++)
        {
            Instantiate(npcCollectionSS.Players[i].PlayerReference, npcCollectionSS.Players[i].SpawnPoint, Quaternion.identity);
        }
    }
}
