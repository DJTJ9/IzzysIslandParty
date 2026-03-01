using System;
using Player;
using Player.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfHole : MonoBehaviour
{
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersSO;

    public static event Action<int> onMinigolfPlayerFinished;
    [SerializeField] private UnityEvent onGameEnd;
    
    private const int MAX_PLAYER_COUNT = 4;
    private int m_finishedPlayers;

    private void OnEnable()
    {
        m_finishedPlayers = 0;
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Controller>(out var controller))
        {
            onMinigolfPlayerFinished?.Invoke(controller.PlayerIndex);
            controller.DisableController();
            
            ++m_finishedPlayers;
            ConsoleProDebug.LogToFilter($"Players finished: {m_finishedPlayers}", "Debug");
            
            if (m_finishedPlayers / 2 == MAX_PLAYER_COUNT- 1)
            {
                //TODO: Start countdown for last player to finish
                
                onGameEnd.Invoke();
                ConsoleProDebug.LogToFilter("Game End!", "Event");
            }
        }
        
        if (_other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var _playerController))
        {
            _playerController.SwitchToUIInputMap();
        }
        
        _other.transform.parent.GetComponentInChildren<PlayerUIMinigolfMayhem>().StopTimer();
            
        // _other.gameObject.SetActive(false);
    }
}
