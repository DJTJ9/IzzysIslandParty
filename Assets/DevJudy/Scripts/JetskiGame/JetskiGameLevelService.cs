using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using Audio;
using ImprovedTimers;
using MultiuseScripts;
using Pathfinding;
using TMPro;
using UnityEngine;

namespace JetskiGame
{
    public class JetskiGameLevelService : RacingGameLevelService
    {
        #region consts

        private const string levelStartText = "START";
        private const string dotsText = "...";
        private const string unfinishedRaceText = "--:--:--";

        private const int maxNumberOfPlayers = 5;

        #endregion

        [Header("Level start/end: ")]
        [SerializeField] private int secondsToStartLevel;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private TextMeshProUGUI levelCountdownText;
        [SerializeField] private int secondsToEndLevel = 10;
        private CountdownTimer endLevelTimer;
        
        private bool raceStarted;
        private bool raceEnded;

        [Header("Temp ")]
        // !! The text belongs in another class
        [SerializeField] private TextMeshProUGUI placementText;
        [SerializeField] private TextMeshProUGUI onFinishLineCrossedText;
        [SerializeField] private GameAudioManager audioManager;

        private void Awake()
        {
            WinnerPlacementOrder =  new Dictionary<int, Tuple<GameObject, string>>();
            endLevelTimer = new CountdownTimer(secondsToEndLevel);
            
            CheckPlacementList();
        }

        private void CheckPlacementList()
        {
            if (placementOrder.Length > maxNumberOfPlayers)
            {
                Debug.LogWarning($"PlacementOrder array is more than maximum number of players ({maxNumberOfPlayers}), resizing array");
                placementOrder = ArrayHelper.ResizeArray(placementOrder, maxNumberOfPlayers);
            }

            for (int i = placementOrder.Length - 1; i > 0; i--)
            {
                if (placementOrder[i] == null)
                {
                    Debug.LogWarning($"PlacementOrder[{i}] is empty, resizing array");
                    ArrayHelper.RemoveFromArray(placementOrder, placementOrder[i]);
                }
            }
        }

        private void Start()
        {
            if (audioManager != null)
                audioManager.StartBackgroundMusic(() => !raceEnded);

            if (goalTransform == null)
            {
                Debug.LogWarning("Goal transform not set");
                checkPlacements = false;
            }

            StartLevel();
        }

        public override void StartLevel()
        {
            StartCoroutine(CountdownToLevelStart());
        }

        private void LetNPCsStart()
        {
            foreach (GameObject obj in placementOrder)
            {
                if (obj.TryGetComponent(out PathfindingUnit pathfindingUnit))
                    pathfindingUnit.CanFollowPath();
            }
        }

        private void OnCoroutineOver()
        {
            StopCoroutine(CountdownToLevelStart());

            LetNPCsStart();

            OnLevelStart.Invoke();
            raceStarted = true;
        }

        private IEnumerator CountdownToLevelStart()
        {
            if (levelCountdownText == null)
                OnCoroutineOver();

            levelCountdownText.enabled = true;

            for (int i = secondsToStartLevel; i > 0; i--)
            {
                levelCountdownText.text = i + dotsText;

                yield return new WaitForSecondsRealtime(1f);
            }

            levelCountdownText.text = levelStartText;

            yield return new WaitForSecondsRealtime(1f);

            levelCountdownText.enabled = false;

            OnCoroutineOver();

            yield return null;
        }

        private void FixedUpdate()
        {
            if (raceStarted && checkPlacements)
                CheckRacerPlacements();
        }

        private void CheckRacerPlacements()
        {
            for (int i = 1; i < placementOrder.Length; i++)
            {
                float currentDistanceToCompare = GetDistanceToGoal(placementOrder[i].transform.position);
                GameObject currentGameObjectBeingCompared = placementOrder[i];

                int leftNeighbour = i - 1; // 0, 1

                // While the leftNeighbour is not out of bounds, and the ln distance is more than the currentDistance
                while (leftNeighbour >= 0 && GetDistanceToGoal(placementOrder[leftNeighbour].transform.position) > currentDistanceToCompare)
                {
                    // Set the index [ln + 1] (first pass index of current key), to be the value of [ln]
                    placementOrder[leftNeighbour + 1] = placementOrder[leftNeighbour];
                    // Then go further down the array and look at the next ln
                    leftNeighbour = leftNeighbour - 1;
                }

                // If the distance is less, or all left neighbours have been checked, make sure to give the key value to the current index (ln + 1)
                placementOrder[leftNeighbour + 1] = currentGameObjectBeingCompared;
            }

            var placement = GetPlayerNumber() + 1;

            placementText.text = (placement.ToString() + "/" + placementOrder.Length);
        }

        private int GetPlayerNumber()
        {
            for (int i = 0; i < placementOrder.Length; i++)
            {
                if (placementOrder[i].CompareTag("Player"))
                    return i;
            }

            return -1;
        }

        private float GetDistanceToGoal(Vector3 _gameObjectPos)
        {
            return Vector2.Distance(new Vector2(_gameObjectPos.x, _gameObjectPos.z), new Vector2(goalTransform.position.x, goalTransform.position.z));
        }

        public override void OnFinishLineCrossed(GameObject _triggeringObj)
        {
            int placement = WinnerPlacementOrder.Count + 1;
            var currentObj = new Tuple<GameObject, string>(_triggeringObj, LevelTimer.Instance.GetTimeAsString());
            WinnerPlacementOrder.Add(placement, currentObj);
            
            if (((1 << _triggeringObj.layer) & playerLayerMask) != 0)
            {
                OnPlayerCrossedFinishLine();
                
                StartCoroutine(StartLevelCountdownTimer());
            }
        }

        private void OnPlayerCrossedFinishLine()
        {
            if (onFinishLineCrossedText != null)
                onFinishLineCrossedText.gameObject.SetActive(true);
        }

        private IEnumerator StartLevelCountdownTimer()
        {
            endLevelTimer.Start();
            
            while (endLevelTimer.IsRunning)
            {
                yield return new WaitForFixedUpdate();
            }

            CheckWinnerPlacementList();
            
            raceEnded = true;
            EndLevel();

            yield return null;
        }

        private void CheckWinnerPlacementList()
        {
            if (WinnerPlacementOrder.Count < maxNumberOfPlayers)
            {
                for (int i = WinnerPlacementOrder.Count; i < maxNumberOfPlayers; i++)
                {
                    WinnerPlacementOrder.Add(WinnerPlacementOrder.Count + 1, new Tuple<GameObject, string>(placementOrder[i], unfinishedRaceText));
                }
            }
        }

        public override void EndLevel()
        {
            LevelTimer.Instance.EndTimerAndDisplayFinishTime();
            
            onFinishLineCrossedText?.gameObject.SetActive(true);
            raceStarted = false;
            
            OnLevelEnd?.Invoke();
        }
    }
}