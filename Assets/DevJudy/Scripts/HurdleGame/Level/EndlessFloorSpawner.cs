using System.Collections.Generic;
using ImprovedTimers;
using UnityEngine;

namespace HurdleGame
{
    public class EndlessFloorSpawner : MonoBehaviour
    {
        [Header("Level variables: ")]
       // [SerializeField] private float levelDurationInSeconds = 120;

        private CountdownTimer timerUntilGoalSpawns;

        [Header("Movement variables: ")]
        [SerializeField] private Vector3 moveDir;

        [SerializeField] private float spawnFinishLineAddition;
        [SerializeField] private float spawnLaneAddition;

        [Header("Objects: ")]
        [SerializeField] private ObstacleSpawner obstacleSpawner;

        [SerializeField] private GameObject finishLine;
        [SerializeField] private List<GameObject> lanes;
        private List<List<GameObject>> laneFloors;

        [SerializeField] private List<CustomTriggerBehaviour> triggerBehaviours;

        private void Awake()
        {
            SetUpLaneFloors();
        }

        private void SetUpLaneFloors()
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

        private void SetUpTriggerBehaviours()
        {
            if (triggerBehaviours == null || triggerBehaviours.Count < 1)
                Debug.LogError("No TriggerBehaviours Found");
            else
            {
                foreach (CustomTriggerBehaviour triggerBehaviour in triggerBehaviours)
                {
                    //triggerBehaviour.EnteredTriggerAction += obstacleSpawner.OnPassedCollider;
                    triggerBehaviour.EnteredTriggerAction += OnPassThroughCollider;
                }
            }
        }

        private void Start()
        {
            SetUpTriggerBehaviours();

            //timerUntilGoalSpawns = new CountdownTimer(levelDurationInSeconds);
            //timerUntilGoalSpawns.OnTimerStop += SpawnInFinishLine;

            //timerUntilGoalSpawns.Start();
        }

        // !! Dont make it lane base, just update all lanes simultaneously
        private void OnPassThroughCollider(Collider _collider)
        {
            //!! Maybe make queue

            // Get the lane that the collider is on
            // Get whatever floor is the last in that lane
            // thatLane.transform.position += new Vector3(spawnLaneAddition, thatLane.transform.position.y, thatLane.transform.position.z);
        }

        private void SpawnInFinishLine()
        {
            if (finishLine == null)
                return;

            Debug.Log("Adding finish line");
            finishLine.SetActive(true);
            // Vector3.x = furthest line forward + spawnFinishLineAddition
            finishLine.transform.position = new Vector3(spawnFinishLineAddition, finishLine.transform.position.y, finishLine.transform.position.z);
        }
    }
}