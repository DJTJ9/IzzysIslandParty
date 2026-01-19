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
        [SerializeField] private List<Rigidbody> objectsToMove;

        [SerializeField] private Rigidbody finishLine;

        private void Start()
        {
            timerUntilGoalSpawns = new CountdownTimer(levelDurationInSeconds);
            timerUntilGoalSpawns.OnTimerStop += SpawnInFinishLine;

            timerUntilGoalSpawns.Start();
        }

        private void OnTriggerExit(Collider _obj)
        {
            _obj.transform.position = new Vector3(spawnPositionX, _obj.transform.position.y, _obj.transform.position.z);
        }
        
        private void FixedUpdate()
        {
            for (var index = 0; index < objectsToMove.Count; index++)
            {
                var rb = objectsToMove[index];
                
                rb.linearVelocity = moveDir * (100 * Time.fixedDeltaTime);
            }
        }

        private void SpawnInFinishLine()
        {
            Debug.Log("Adding finish line");
            finishLine.transform.position = new Vector3(spawnPositionX, finishLine.transform.position.y, finishLine.transform.position.z);
            objectsToMove.Add(finishLine);
        }
    }
}