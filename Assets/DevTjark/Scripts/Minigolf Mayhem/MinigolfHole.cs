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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (other.TryGetComponent<Controller>(out var controller))
        {
            onMinigolfPlayerFinished?.Invoke(controller.PlayerIndex);
            controller.DisableController();
            
            ++m_finishedPlayers;
            
            if (m_finishedPlayers == MAX_PLAYER_COUNT - 1)
            {
                onGameEnd.Invoke();
                ConsoleProDebug.LogToFilter("Game End!", "Event");
            }
        }
        
        if (other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var playerController))
        {
            playerController.SwitchToUIInputMap();
        }
        
        other.transform.parent.GetComponentInChildren<PlayerUIMinigolfMayhem>().StopTimer();
            
        // other.gameObject.SetActive(false);
    }
}
