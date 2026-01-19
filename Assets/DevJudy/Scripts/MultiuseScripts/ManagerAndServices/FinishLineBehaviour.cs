using System.Collections;
using HelperScripts;
using ImprovedTimers;
using Service;
using UnityEngine;

namespace MultiuseScripts
{
    public class FinishLineBehaviour : MonoBehaviour
    {
        [Header("Dependencies: ")]
        [SerializeField] private CustomTriggerBehaviour finishLineTrigger;

        // Change that and ILevelService to a levelService parent 
        [SerializeField] private JetskiGameLevelService levelService;
        private CountdownTimer endLevelTimer;

        [Header("Variables: ")]
        [SerializeField] private int secondsToEndLevel = 10;

        [SerializeField] private LayerMask playerLayerMask;
        [SerializeField] private bool checkWinners;

        // Temp!!
        // Maybe this should be a dictionary? To save the obj, placement AND time?
        private GameObject[] placementList = { null, null, null, null, null };

        private void Start()
        {
            if (finishLineTrigger != null)
                finishLineTrigger.EnteredTriggerAction += OnFinishLineEntered;
            else
                Debug.LogWarning("No finishLine trigger");

            endLevelTimer = new CountdownTimer(secondsToEndLevel);
        }

        private void OnFinishLineEntered(Collider _triggeringObj)
        {
            //!! Visual feedback!!

            // Keep track of ppl crossing (1, 2, 3- place)
            // maybe levelService.winnerList
            // or maybe not and use whatever will keep track of the placements during the race

            if (checkWinners)
            {
                // ?? Is this obsolete?
                ArrayHelper.AddToArray(placementList, _triggeringObj.gameObject);
                // send winner to levelService
            }

            if (((1 << _triggeringObj.gameObject.layer) & playerLayerMask) != 0)
            {
                levelService?.OnFinishLineCrossed();

                StartCoroutine(StartLevelCountdownTimer());
                // !! Disable controls already
            }
        }

        private IEnumerator StartLevelCountdownTimer()
        {
            endLevelTimer.Start();

            while (endLevelTimer.IsRunning)
            {
                yield return new WaitForFixedUpdate();
            }

            EndLevel();

            yield return null;
        }

        private void EndLevel()
        {
            levelService.EndLevel();
        }
    }
}