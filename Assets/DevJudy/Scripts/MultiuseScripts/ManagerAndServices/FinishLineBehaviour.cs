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
            //!! Visual feedback!!
            
           levelService?.OnFinishLineCrossed(_triggeringObj.gameObject);
        }
    }
}