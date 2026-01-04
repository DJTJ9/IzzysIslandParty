using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts
{
    class EndlessFloorLinear : MonoBehaviour
    {
        [SerializeField] private List<GameObject> floorObjects;
        [SerializeField] private Transform playerPosition;

        [SerializeField] private float floorUpdateThreshold;
        [SerializeField] private float floorSpawnDistance;

        [Header("Debug")]
        [SerializeField] private int closestFloorIndex = 0;

        [SerializeField] private int furthestFloorIndex;

        [SerializeField] private Vector3 prevPlayerPosition;

        private Vector3 floorSpawnAddition;

        private void Start()
        {
            floorSpawnAddition = new Vector3(0, 0, floorSpawnDistance);

            furthestFloorIndex = floorObjects.Count - 1;
            prevPlayerPosition = playerPosition.position;
        }

        private void FixedUpdate()
        {
            if (playerPosition.position.z - prevPlayerPosition.z >= floorUpdateThreshold)
                MoveFloorForward();
            else if (playerPosition.position.z - prevPlayerPosition.z <= -floorUpdateThreshold)
                MoveFloorBackward();
        }

        private void MoveFloorForward()
        {
            floorObjects[closestFloorIndex].transform.position = floorObjects[furthestFloorIndex].transform.position + floorSpawnAddition;

            closestFloorIndex = (closestFloorIndex == floorObjects.Count - 1) ? 0 : closestFloorIndex + 1;
            furthestFloorIndex = (furthestFloorIndex == floorObjects.Count - 1) ? 0 : furthestFloorIndex + 1;

            prevPlayerPosition = playerPosition.position;
        }

        private void MoveFloorBackward()
        {
            floorObjects[furthestFloorIndex].transform.position = floorObjects[closestFloorIndex].transform.position - floorSpawnAddition;

            closestFloorIndex = (closestFloorIndex == 0) ? floorObjects.Count - 1 : closestFloorIndex - 1;
            furthestFloorIndex = (furthestFloorIndex == 0) ? floorObjects.Count - 1 : furthestFloorIndex - 1;

            prevPlayerPosition = playerPosition.position;
        }
    }
}