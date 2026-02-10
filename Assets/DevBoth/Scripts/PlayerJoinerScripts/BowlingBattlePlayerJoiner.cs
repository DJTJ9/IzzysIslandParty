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

    private void Start()
    {
        m_playerIndex = 0;
        m_npcIndex = 0;
        currentPlayers.Players.Clear();
    }

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
    
    public void JoinNPCsBB()
    {
        for (var i = m_playerIndex - 1; i < npcCollectionBB.Players.Count; i++)
        {
            Instantiate(npcCollectionBB.Players[i].PlayerReference, npcCollectionBB.Players[i].SpawnPoint, Quaternion.identity);
        }
    }
}
