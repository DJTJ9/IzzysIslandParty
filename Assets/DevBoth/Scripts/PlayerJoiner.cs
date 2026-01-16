using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerJoiner : MonoBehaviour
{
    public Transform[] SpawnPoints;
    public GameObject[] Players;

    private void Awake()
    {
        SpawnPlayer(0);
        SpawnPlayer(1);
        // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
        // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    }
    
    [Button]
    public void SpawnPlayer(int playerIndex)
    {
        Instantiate(Players[playerIndex], SpawnPoints[playerIndex].position, SpawnPoints[playerIndex].rotation);
    }
}
