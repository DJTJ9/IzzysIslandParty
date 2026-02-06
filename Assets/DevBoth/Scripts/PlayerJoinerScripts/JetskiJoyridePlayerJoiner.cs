using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class JetskiJoyridePlayerJoiner : MonoBehaviour
{
    [SerializeField] private SO_PlayerCollection currentPlayers;
    
    [FoldoutGroup("Jetski Joyride", expanded: true)]
    public SO_PlayerCollection playerCollectionJJ;

    private int m_playerIndex = 0;
    private int m_npcIndex = 0;

    private void Start()
    {
        m_playerIndex = 0;
        m_npcIndex = 0;
        currentPlayers.Players.Clear();
    }

    public void PlayerJoinedJJ(PlayerInput _playerInput)
    {
        _playerInput.gameObject.transform.position = playerCollectionJJ.Players[m_playerIndex].SpawnPoint;
        ++m_playerIndex;
    }
}
