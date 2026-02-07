using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SwaggySnapshotsPlayerJoiner : MonoBehaviour
{
    [SerializeField] private SO_PlayerCollection currentPlayers;
    
    [FoldoutGroup("Scriptable Objects", expanded: true)]
    [SerializeField] private SO_PlayerCollection playerCollectionSS;
    [SerializeField] private SO_PlayerCollection npcCollectionSS;

    private int m_playerIndex = 0;
    private int m_npcIndex = 0;

    private void Start()
    {
        m_playerIndex = 0;
        m_npcIndex = 0;
        currentPlayers.Players.Clear();
    }

    public void PlayerJoined(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out NPC_SwaggySnapshots npc))
        {
            npc.SetPlayerIndex(m_playerIndex);
            currentPlayers.Players.Add(npcCollectionSS.Players[npc.GetPlayerIndex() - 1]);
            ++m_playerIndex;
            ++m_npcIndex;
            return;
        }

        playerCollectionSS.Players[m_playerIndex].InitializePlayer(_playerInput.gameObject, playerCollectionSS.Players[m_playerIndex], m_playerIndex);
        currentPlayers.Players.Add(playerCollectionSS.Players[m_playerIndex]);
        ++m_playerIndex;
    }
    
    public void JoinNPCs()
    {
        var nPCStartIndex = m_playerIndex - 1;
        for (var i = nPCStartIndex; i < npcCollectionSS.Players.Count; i++)
        {
            Instantiate(npcCollectionSS.Players[i].PlayerPrefab, npcCollectionSS.Players[i].SpawnPoint, Quaternion.identity);
        }
    }
}
