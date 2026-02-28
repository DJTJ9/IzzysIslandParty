using JetskiGame;
using Pathfinding;
using UnityEngine;

namespace MultiuseScripts
{
    public class FinishLineBehaviour : MonoBehaviour
    {
        [Header("Dependencies: ")]
        [SerializeField] private CustomTriggerBehaviour finishLineTrigger;

        // Change that and ILevelService to a levelService parent 
        [SerializeField] private RacingGameLevelService levelService;

        private void Start()
        {
            if (finishLineTrigger != null)
                finishLineTrigger.EnteredTriggerAction += OnFinishLineEntered;
            else
                Debug.LogWarning("No finishLine trigger");
        }

        private void OnFinishLineEntered(Collider _triggeringObj)
        {
            if (_triggeringObj.gameObject.CompareTag("Ignore") || !_triggeringObj.gameObject.TryGetComponent(out Controller controller))
                return;
            //!! Visual feedback!!
            if (controller is JetskiController playerController)
                playerController.OnObstacleCleared();
            else if (controller is JetskiNPCBehaviour npcController)
                npcController.OnObstacleCleared();

            levelService?.OnFinishLineCrossed(_triggeringObj.gameObject);
        }
    }
}