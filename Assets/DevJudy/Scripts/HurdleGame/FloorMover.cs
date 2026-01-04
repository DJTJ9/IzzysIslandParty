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
        [SerializeField] private List<GameObject> objectsToMove;

        [SerializeField] private GameObject finishLine;

        private void Start()
        {
            timerUntilGoalSpawns = new CountdownTimer(levelDurationInSeconds);
            timerUntilGoalSpawns.OnTimerStop += SpawnInFinishLine;

            timerUntilGoalSpawns.Start();
        }

        private void OnTriggerExit(Collider _obj)
        {
            if (!_obj.CompareTag("Player"))
                _obj.transform.position = new Vector3(spawnPositionX, _obj.transform.position.y, _obj.transform.position.z);
        }

        private void FixedUpdate()
        {
            foreach (GameObject lane in objectsToMove)
                lane.transform.position += moveDir * Time.fixedDeltaTime;
        }

        private void SpawnInFinishLine()
        {
            Debug.Log("Adding finish line");
            finishLine.transform.position = new Vector3(spawnPositionX, finishLine.transform.position.y, finishLine.transform.position.z);
            objectsToMove.Add(finishLine);
        }
    }
}