using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnerMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    private int m_playerCount;

    // public void PlayerJoinedBB(PlayerInput playerInput)
    // {
    //     playerInput.transform.position = spawnPoints[m_playerCount].transform.position;
    //     
    //     ++m_playerCount;
    // }
}
