using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using ImprovedTimers;
using UnityEngine;

public abstract class FinishLineBehaviour : MonoBehaviour
{
    [Header("Dependencies: ")]
    [SerializeField] private CustomTriggerBehaviour finishLineTrigger;

    [SerializeField] private LevelService levelService;
    private CountdownTimer endLevelTimer;

    [Header("Variables: ")]
    [SerializeField] private int secondsToEndLevel = 10;

    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private bool checkWinners;

    // Temp!!
    private const int maxPlayers = 5;
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
        // Keep track of ppl crossing (1, 2, 3- place)
        // maybe levelService.winnerList
        // or maybe not and use whatever will keep track of the placements during the race
        if (checkWinners)
        {
            ArrayHelper.AddToArray(placementList, _triggeringObj.gameObject);
            Debug.Log(_triggeringObj.name + " has finished " + placementList + "/" + placementList.Length);
        }

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