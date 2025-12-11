using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private bool checkWinners;

    [field: SerializeField] private List<GameObject> winnerList = new List<GameObject>();

    [SerializeField] private string playerTagName = "Player";
    [SerializeField] private string npcTagName = "NPC";
    
    private void OnTriggerEnter(Collider _collidingObj)
    {
        if (checkWinners && (_collidingObj.CompareTag(playerTagName) || _collidingObj.CompareTag(npcTagName)))
        {
            winnerList.Add(_collidingObj.gameObject);
        }
        
        if (_collidingObj.CompareTag(playerTagName))
        {
            if (checkWinners)
                winnerList.Add(_collidingObj.gameObject);
            
            // Check at which place the other players and set first, second and third place accordingly
            gameOverManager?.SetGameOver();
            Debug.Log("GAME OVER ----------");
        }
    }
}