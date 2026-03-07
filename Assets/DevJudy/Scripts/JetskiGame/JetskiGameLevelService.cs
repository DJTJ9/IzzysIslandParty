using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using Audio;
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

        private const int maxNumberOfPlayers = 4;

        #endregion

        [Header("Level start/end: ")]
        [SerializeField] private int secondsToStartLevel;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private TextMeshProUGUI levelCountdownText;
        [SerializeField] private int secondsToEndLevel = 30;
        [SerializeField] private int showCountdownSeconds = 10;
        private List<Tuple<GameObject, string>> winnerPlacementOrder;
        private List<float> finishingTimes;

        private bool raceStarted;
        private bool raceEnded;

        [Header("Dependencies: ")]
        [SerializeField] private TextMeshProUGUI raceCountdownText;
        [SerializeField] private GameAudioManager audioManager;
        [SerializeField] private LevelTimer levelTimer;
        private Coroutine endLevelCoroutine;

        private void Awake()
        {
            winnerPlacementOrder = new List<Tuple<GameObject, string>>();
            finishingTimes = new List<float>();

            if (checkPlacements)
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
        }

        public override void OnPlayerJoined(GameObject _player)
        {
            if (placementOrder == null || placementOrder.Length < 1)
                placementOrder = new GameObject[maxNumberOfPlayers];

            ArrayHelper.AddToArray(placementOrder, _player);
        }

        public override void OnNPCJoined(GameObject _npc)
        {
            ArrayHelper.AddToArray(placementOrder, _npc);
        }

        public override void StartLevel()
        {
            StartCoroutine(CountdownToLevelStart());
        }

        private void LetNPCsStart()
        {
            if (placementOrder == null || placementOrder.Length < 1)
            {
                Debug.LogWarning("PlacementOrder array is empty");
                return;
            }

            for (int i = 0; i < placementOrder.Length; i++)
            {
                if (!placementOrder[i])
                {
                    Debug.LogWarning($"PlacementOrder[{i}] is empty...");
                    continue;
                }

                if (placementOrder[i].TryGetComponent(out JetskiNPCBehaviour pathfindingUnit))
                {
                    pathfindingUnit.SetTarget(goalTransform);
                    pathfindingUnit.CanFollowPath();
                }
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
            for (int i = 1; i < winnerPlacementOrder.Count; i++)
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
                    leftNeighbour = (leftNeighbour - 1);
                }

                // If the distance is less, or all left neighbours have been checked, make sure to give the key value to the current index (ln + 1)
                placementOrder[leftNeighbour + 1] = currentGameObjectBeingCompared;
            }

            SetPlacements();
        }

        private void SetPlacements()
        {
            for (int i = 0; i < placementOrder.Length; i++)
            {
                if (placementOrder[i].gameObject.TryGetComponent(out JetskiController playerController))
                    playerController.SetPlacement(i + 1);
                else if (placementOrder[i].gameObject.TryGetComponent(out JetskiNPCBehaviour npc))
                    npc.SetPlacement(i + 1);
            }
        }

        private float GetDistanceToGoal(Vector3 _gameObjectPos)
        {
            return Vector2.Distance(new Vector2(_gameObjectPos.x, _gameObjectPos.z), new Vector2(goalTransform.position.x, goalTransform.position.z));
        }

        public override void OnFinishLineCrossed(GameObject _triggeringObj)
        {
            levelTimer.GetTime(out int minutes, out int seconds, out int milliseconds);
            GetTotalTimeDeduction(_triggeringObj, out var timeDeductionMinutes, out var timeDeductionSeconds);
            
            int finishingTimeMinutes = minutes + timeDeductionMinutes;
            int finishingTimeSeconds = (seconds + timeDeductionSeconds);

            if (finishingTimeSeconds >= 60f)
            {
                finishingTimeMinutes++;
                finishingTimeSeconds = (finishingTimeMinutes % 60);
            }
            
            finishingTimes.Add(finishingTimeMinutes + (finishingTimeSeconds * 0.010f) + (milliseconds * 0.00010f));
            
            string finishingTime = levelTimer.GetTimeAsString(finishingTimeMinutes, finishingTimeSeconds, milliseconds);

            var currentObj = new Tuple<GameObject, string>(_triggeringObj, finishingTime);
            winnerPlacementOrder.Add(currentObj);

            if (winnerPlacementOrder.Count == 1)
               endLevelCoroutine = StartCoroutine(StartLevelCountdownTimer());
            else if (winnerPlacementOrder.Count == maxNumberOfPlayers)
            {
                StopCoroutine(endLevelCoroutine);
                endLevelCoroutine = null;
                
                OrganizeWinnerPlacementList();
                SetGameScores();

                raceEnded = true;
                EndLevel();
            }
        }

        private void GetTotalTimeDeduction(GameObject _triggeringObj, out int _timeDeductionMinutes, out int _timeDeductionSeconds)
        {
            if (_triggeringObj.TryGetComponent(out JetskiController playerController))
            {
                playerController.OnFinishLineCrosses();
                playerController.GetFinalTimeDeduction(out _timeDeductionMinutes, out _timeDeductionSeconds);
                return;
            }

            if (_triggeringObj.TryGetComponent(out JetskiNPCBehaviour jetskiNPCBehaviour))
            {
                jetskiNPCBehaviour.OnFinishLineCrossed();
                jetskiNPCBehaviour.GetFinalTimeDeduction(out _timeDeductionMinutes, out _timeDeductionSeconds);
                return;
            }

            _timeDeductionMinutes = 0;
            _timeDeductionSeconds = 0;
        }
        
        private IEnumerator StartLevelCountdownTimer()
        {
            int countdown = secondsToEndLevel;

            while (countdown > 0)
            {
                countdown--;

                yield return new WaitForSecondsRealtime(1f);

                if (countdown == showCountdownSeconds)
                {
                    Debug.Log("Countdown text active");
                    levelCountdownText.enabled = true;
                }
                
                if (countdown <= showCountdownSeconds)
                    levelCountdownText.text = countdown.ToString() + "...";
            }
            
            Debug.Log("StartLevelCountdownTimer");
            
            OrganizeWinnerPlacementList();
            SetGameScores();

            raceEnded = true;
            EndLevel();

            yield return null;
        }

        private void OrganizeWinnerPlacementList()
        {
            CheckIfWinnerPlacementListFull();

            for (int i = 1; i < winnerPlacementOrder.Count; i++)
            {
                float currentTimePenaltyToCompare = finishingTimes[i];
                var currentGameObjectBeingCompared = winnerPlacementOrder[i];

                int leftNeighbour = i - 1;

                // While the leftNeighbour is not out of bounds, and the ln distance is more than the currentDistance
                while (leftNeighbour >= 0 && finishingTimes[leftNeighbour] > currentTimePenaltyToCompare)
                {
                    // Set the index [ln + 1] (first pass index of current key), to be the value of [ln]
                    winnerPlacementOrder[leftNeighbour + 1] = winnerPlacementOrder[leftNeighbour];
                    // Then go further down the array and look at the next ln
                    leftNeighbour = (leftNeighbour - 1);
                }

                // If the distance is less, or all left neighbours have been checked, make sure to give the key value to the current index (ln + 1)
                winnerPlacementOrder[leftNeighbour + 1] = currentGameObjectBeingCompared;
            }
        }

        private void CheckIfWinnerPlacementListFull()
        {
            if (winnerPlacementOrder.Count < maxNumberOfPlayers)
            {
                CheckRacerPlacements();
                
                for (int i = 0; i < maxNumberOfPlayers; i++)
                {
                    var foundObjectInList = false;

                    for (int j = 0; j < winnerPlacementOrder.Count; j++)
                    {
                        if (placementOrder[i] == winnerPlacementOrder[j].Item1)
                        {
                            foundObjectInList = true;
                        }
                    }

                    if (!foundObjectInList)
                    {
                        winnerPlacementOrder.Add(new Tuple<GameObject, string>(placementOrder[i], unfinishedRaceText));
                        finishingTimes.Add(1000f + i);
                    }
                }
            }
        }

        private void SetGameScores()
        {
            for (int i = 0; i < winnerPlacementOrder.Count; i++)
            {
                if (winnerPlacementOrder[i].Item1.gameObject.TryGetComponent(out JetskiController playerController))
                {
                    playerController.SetPlacement(i + 1);
                    playerController.SetTime(winnerPlacementOrder[i].Item2);
                }
                else if (winnerPlacementOrder[i].Item1.gameObject.TryGetComponent(out JetskiNPCBehaviour npcController))
                {
                    npcController.SetPlacement(i + 1);
                    npcController.SetTime(winnerPlacementOrder[i].Item2);
                }
            }
            Debug.Log("SetGameScores");
        }

        public override void EndLevel()
        {
            Debug.Log("-EndLevel-");
            levelTimer.EndTimerAndDisplayFinishTime();
            raceStarted = false;

            OnLevelEnd?.Invoke();
        }
    }
}