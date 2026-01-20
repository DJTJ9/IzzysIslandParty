using System;
using System.Collections.Generic;
using ImprovedTimers;
using UnityEngine;

namespace HurdleGame
{
    public class FloorMover : MonoBehaviour
    {
        [Header("Level variables: ")]
        [SerializeField] private float levelDurationInSeconds;

        private CountdownTimer timerUntilGoalSpawns;

        [Header("Movement variables: ")]
        [SerializeField] private Vector3 moveDir;

        private Vector3 colliderMoveDir;

        [SerializeField] private float spawnPositionX;

        [Header("Objects: ")]
        [SerializeField] private List<Rigidbody> objectsToMove; // !! Find better way to move finishLine
        [SerializeField] private List<GameObject> lanes; 
        private List<List<GameObject>> laneFloors; // Game objects: List<lanes[1]>: children, lane 2, lane 3, lane 4
        [SerializeField] private Rigidbody finishLine;

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

        private void OnTriggerExit(Collider _obj)
        {
            // Whenever the last (aka all four) character passes through a floorCollider, place the last collider at the front
            // and make the current one the last one
            // Or check each lane separately and only move that lane with custom trigger behaviour...
            
            _obj.transform.position = new Vector3(spawnPositionX, _obj.transform.position.y, _obj.transform.position.z);
        }
        
      //  private void FixedUpdate()
      //  {
      //      for (var index = 0; index < objectsToMove.Count; index++)
      //      {
      //          var rb = objectsToMove[index];
      //          
      //          rb.linearVelocity = moveDir * (100 * Time.fixedDeltaTime);
      //      }
      //  }
//
        private void SpawnInFinishLine()
        {
            Debug.Log("Adding finish line");
            finishLine.transform.position = new Vector3(spawnPositionX, finishLine.transform.position.y, finishLine.transform.position.z);
            objectsToMove.Add(finishLine);
        }
    }
}