using System;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfHole : MonoBehaviour
{
    [SerializeField] private SO_Placing placingSO;

    [SerializeField] private UnityEvent playerFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        other.TryGetComponent<Controller>(out var controller);
        {
            placingSO.AddPlayerToPlacingList(controller.PlayerIndex);
            playerFinished.Invoke();
            controller.DisableController();
            ConsoleProDebug.LogToFilter($"Player {controller.PlayerIndex} finished at {placingSO.playerPlacing.Count} place!", "Event");
        }
        
        other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var playerController);
        {
            playerController.SwitchToUIInputMap();
        }
            
        // other.gameObject.SetActive(false);
    }
}
