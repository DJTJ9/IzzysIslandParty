using Player.Collections;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigolfMayhemPlayerJoiner : MonoBehaviour
{
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayers;
    
    [FoldoutGroup("Minigolf Mayhem", expanded: true)]
    [SerializeField] private SO_PlayerCollectionRacingGames playerCollectionMM;
    [SerializeField] private SO_PlayerCollectionRacingGames npcCollectionMM;

    private int m_playerIndex = 0;

    private void Start()
    {
        m_playerIndex = 0;
        currentPlayers.Players.Clear();
    }

    public void PlayerJoinedMM(PlayerInput _playerInput)
    {
        if (_playerInput.gameObject.TryGetComponent(out GoapRigidbodyMovement npc))
        {
            npc.SetPlayerIndex(m_playerIndex);
            _playerInput.transform.GetComponentInChildren<CinemachineInputAxisController>().PlayerIndex = npc.GetPlayerIndex();
            currentPlayers.Players.Add(npcCollectionMM.Players[npc.GetPlayerIndex() - 1]);
            _playerInput.gameObject.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            ++m_playerIndex;
            return;
        }

        if (_playerInput.gameObject.TryGetComponent(out PlayerControllerMinigolfMayhem controller))
        {
            controller.SetPlayerIndex(m_playerIndex);
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
    
    public void JoinNPCsMM()
    {
        var nPCStartIndex = m_playerIndex - 1;
        for (var i = nPCStartIndex; i < npcCollectionMM.Players.Count; i++)
        {
            // Instantiate(npcCollectionMM.Players[i].PlayerReference, npcCollectionMM.Players[i].SpawnPoint, Quaternion.identity);
            var npc = PlayerInput.Instantiate(npcCollectionMM.Players[i].PlayerReference, m_playerIndex, null, pairWithDevice: new InputDevice());
            npc.transform.position = npcCollectionMM.Players[i].SpawnPoint;
        }
    }
}
