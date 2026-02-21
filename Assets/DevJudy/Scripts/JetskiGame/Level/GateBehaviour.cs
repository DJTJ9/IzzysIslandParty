using System.Collections.Generic;
using Helper;
using MultiuseScripts;
using Pathfinding;
using UIScripts;
using UnityEngine;

namespace JetskiGame
{
    public class GateBehaviour : MonoBehaviour
    {
        private const int maxPlayers = 4;

        [SerializeField] private CustomTriggerBehaviour middleCollider;
        [SerializeField] private CustomTriggerBehaviour leftCollider;
        [SerializeField] private CustomTriggerBehaviour rightCollider;

        [SerializeField] private FloatReference timeDeduction;

        [SerializeField] private List<bool> clearedGate = new List<bool>();

        private void Start()
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                clearedGate.Add(false);
            }

            middleCollider.EnteredTriggerAction += OnMiddleGateEnter;

            leftCollider.EnteredTriggerAction += OnSideGatesEnter;
            rightCollider.EnteredTriggerAction += OnSideGatesEnter;
        }

        private void OnMiddleGateEnter(Collider _other)
        {
            _other.gameObject.TryGetComponent(out Controller controller);
            int index = controller.PlayerIndex;

            if (clearedGate[index])
                return;

            clearedGate[index] = true;

            if (controller is JetskiController playerController)
                playerController.OnObstacleCleared();
            else if (controller is JetskiNPCBehaviour npcController)
                npcController.OnObstacleCleared();
        }

        private void OnSideGatesEnter(Collider _other)
        {
            _other.gameObject.TryGetComponent(out Controller controller);
            int index = controller.PlayerIndex;

            if (clearedGate[index])
                return;

            clearedGate[index] = true;

            if (controller is JetskiController playerController)
            {
                playerController.OnObstacleMissed(timeDeduction.Value, out var timeDeductionMinutes, out var timeDeductionSeconds);
              
                 UITimerManager timerManager = playerController.gameObject.transform.parent.GetComponentInChildren<UITimerManager>();
              
                timerManager?.UpdateTimerPenaltyText(timeDeductionMinutes, timeDeductionSeconds);
                StartCoroutine(timerManager?.TimeDeductionFeedback());
            }
            else if (controller is JetskiNPCBehaviour npcController)
                npcController.OnObstacleMissed(timeDeduction.Value, out var timeDeductionMinutes, out var timeDeductionSeconds);
        }
    }
}