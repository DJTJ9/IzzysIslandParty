using MultiuseScripts;
using UnityEngine;

namespace JetskiGame
{
    public class GateBehaviour : MonoBehaviour
    {
        [SerializeField] private LevelTimer timer;

        [SerializeField] private CustomTriggerBehaviour middleCollider;
        [SerializeField] private CustomTriggerBehaviour leftCollider;
        [SerializeField] private CustomTriggerBehaviour rightCollider;

        [SerializeField] private FloatReference timeDeduction;

        [SerializeField] private bool clearedGate;

        private void Start()
        {
            middleCollider.EnteredTriggerAction += OnMiddleGateEnter;

            leftCollider.EnteredTriggerAction += OnSideGatesEnter;
            rightCollider.EnteredTriggerAction += OnSideGatesEnter;
        }

        private void OnMiddleGateEnter(Collider _other)
        {
            if (!clearedGate)
            {
                clearedGate = true;
                //!! Give visual feedback via icons
            }
        }

        private void OnSideGatesEnter(Collider _other)
        {
            if (!clearedGate)
            {
                clearedGate = true;

                // !! Give visual feedback via icons

                timer?.DeduceTime(timeDeduction.Value);
            }
        }
    }
}