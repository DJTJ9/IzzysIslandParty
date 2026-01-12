using System;
using UnityEngine;

public class PlayerJoiner : MonoBehaviour
{
    public Transform SpawnPoint1, Spawnpoint2, Spawnpoint3;
    public GameObject Player1, Player2, Player3;

    private void Awake()
    {
        Instantiate(Player1, SpawnPoint1.position, SpawnPoint1.rotation);
        Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
        // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    }
}
