using System;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfHole : MonoBehaviour
{
    [SerializeField] private SO_Placing placingSO;

    [SerializeField] private UnityEvent playerFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var playerController);
            {
                placingSO.AddPlayerToPlacingList(playerController.PlayerIndex);
                playerFinished.Invoke();
            }
            
            other.gameObject.SetActive(false);
        }
    }
}
