using System;
using System.Collections.Generic;
using System.Linq;
using ImprovedTimers;
using UnityEngine;

namespace HurdleGame
{
    public class EndlessFloorSpawner : MonoBehaviour
    {
        [Header("Level variables: ")]
        [SerializeField] private float levelDurationInSeconds;

        private CountdownTimer timerUntilGoalSpawns;

        [Header("Movement variables: ")]
        [SerializeField] private Vector3 moveDir;

        private Vector3 colliderMoveDir;
        [SerializeField] private float spawnFinishLineAddition;
        [SerializeField] private float spawnLaneAddition;

        [Header("Objects: ")]
        [SerializeField] private Rigidbody finishLine;

        [SerializeField] private List<GameObject> lanes;
        private List<List<GameObject>> laneFloors;
        // !! Move/spawn in obstacles

        private bool spawnedInFinishLine;


        private void Awake()
        {
            if (lanes == null || lanes.Count < 1)
                Debug.LogError("No Lanes Found");
            else
            {
                laneFloors = new List<List<GameObject>>();

                foreach (GameObject lane in lanes)
                {
                    List<GameObject> laneFloor = new List<GameObject>();

                    foreach (Transform child in lane.transform)
                    {
                        laneFloor.Add(child.gameObject);
                    }

                    laneFloors.Add(laneFloor);
                }
            }
        }

        private void Start()
        {
            timerUntilGoalSpawns = new CountdownTimer(levelDurationInSeconds);
            timerUntilGoalSpawns.OnTimerStop += SpawnInFinishLine;

            timerUntilGoalSpawns.Start();
        }

        private void OnPassThroughCollider(Collider _collider)
        {
            int lane = -1;
            bool foundLane = false;


            //!! Maybe make queue
            for (int i = 0; i < laneFloors.Count; i++)
            {
                laneFloors[i].Contains(_collider.gameObject);
            }
          
            // Get the lane that the collider is on
            // Get whatever floor is the last in that lane
            // thatLane.transform.position += new Vector3(spawnLaneAddition, thatLane.transform.position.y, thatLane.transform.position.z);
        }

        private void SpawnInFinishLine()
        {
            Debug.Log("Adding finish line");
            // Vector3.x = furthest line forward + spawnFinishLineAddition
            finishLine.transform.position = new Vector3(spawnFinishLineAddition, finishLine.transform.position.y, finishLine.transform.position.z);

            spawnedInFinishLine = true;
        }
    }
}