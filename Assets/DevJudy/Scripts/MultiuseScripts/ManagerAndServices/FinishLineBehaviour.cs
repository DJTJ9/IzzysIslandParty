using System.Collections;
using ImprovedTimers;
using UnityEngine;

public class FinishLineBehaviour : MonoBehaviour
{
    [SerializeField] private CustomTriggerBehaviour finishLineTrigger;
    [SerializeField] private LevelService levelService;
    private CountdownTimer endLevelTimer;

    [SerializeField] private int secondsToEndLevel;

    [SerializeField] private LayerMask playerLayerMask;

    private void Start()
    {
        finishLineTrigger.EnteredTriggerAction += OnFinishLineEntered;

        endLevelTimer = new CountdownTimer(secondsToEndLevel);
    }

    private void OnFinishLineEntered(Collider _triggeringObj)
    {
        // Keep track of ppl crossing (1, 2, 3- place)
        // maybe levelService.winnerList
        // or maybe not and use whatever will keep track of the placements during the race

        if (((1 << _triggeringObj.gameObject.layer) & playerLayerMask) != 0)
        {
            StartCoroutine(EndLevelTimer());
        }
    }

    private IEnumerator EndLevelTimer()
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
        Debug.Log("End of level");
        
        levelService.EndLevel();
    }
}