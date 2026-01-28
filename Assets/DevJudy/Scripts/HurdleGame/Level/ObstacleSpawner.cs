using System.Collections.Generic;
using UnityEngine;

namespace HurdleGame
{
    // !! Add object pooling
    public class ObstacleSpawner : MonoBehaviour
    {
        private const int amountOfPlayers = 4;

        [SerializeField] private List<GameObject> obstacles;
        [SerializeField] private float spawnDistanceToTrigger = 46f;
        [SerializeField] private float spawnDistanceBetweenObjects = 9f;
        private float spawnPositionZ = -3.9f;
        private int timesObstaclesSpawned = 0;
        private int passedCollider = 0;

        private void Awake()
        {
           //if (obstacles == null || obstacles.Count == 0)
           //{
           //    Debug.LogError("Obstacles list is empty");
           //}
           //else
           //{
           //    for (int i = 0; i < obstacles.Count; i++)
           //    {
           //        obstacles[i].SetActive(false);
           //    }
           //}
        }

        public void OnPassedCollider(Collider _collider)
        {
            if (passedCollider == 0)
                SpawnObstacles(_collider);

            passedCollider++;

            if (passedCollider % amountOfPlayers == 0)
                passedCollider = 0;
        }

        private void SpawnObstacles(Collider _collider)
        {
            var randomObstacleAmountPossible = GetPossibleAmountOfObstaclesToSpawn();
            var obstacleAmountToSpawn = Random.Range(1, randomObstacleAmountPossible + 1);

            var obstaclesToSpawn = GetRandomObstacle(obstacleAmountToSpawn);

            for (int i = 0; i < obstaclesToSpawn.Count; i++)
            {
                var spawnPosition = new Vector3(GetSpawnPositionX(_collider.transform, i), 0, spawnPositionZ);
                obstaclesToSpawn[i].transform.position = spawnPosition;
                obstaclesToSpawn[i].SetActive(true);
            }

            timesObstaclesSpawned++;
        }

        private int GetPossibleAmountOfObstaclesToSpawn()
        {
            switch (timesObstaclesSpawned)
            {
                case 0:
                case 1:
                    return 1;
                case 2:
                case 3:
                    return 2;
                default:
                    return 3;
            }
        }

        private List<GameObject> GetRandomObstacle(int _amountToSpawn)
        {
            var obstaclesToSpawn = new List<GameObject>();
            for (int i = 0; i < _amountToSpawn;)
            {
                var objectToSpawn = obstacles[Random.Range(0, obstacles.Count)];

                if (!obstaclesToSpawn.Contains(objectToSpawn))
                {
                    obstaclesToSpawn.Add(objectToSpawn);
                    i++;
                }
            }

            return obstaclesToSpawn;
        }

        private float GetSpawnPositionX(Transform _collider, int _i)
        {
            return _collider.position.x + spawnDistanceToTrigger + (spawnDistanceBetweenObjects * _i);
        }
    }
}