using System.Collections;
using System.Collections.Generic;
using enums;
using Helper;
using ScriptableObjects;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    [DefaultExecutionOrder(-100)]
    public class FishingSystem : MonoBehaviour
    {
        [Header("Variables: ")]
        private static FishingSystem instance;

        public static FishingSystem Instance => instance;

        [SerializeField] private List<SO_Fish> fishList;
        [SerializeField] private Vector2 secondsUntilFishBiteRange;
        [SerializeField] private Vector2 buttonPressTimerRange;
        [SerializeField] private FloatReference coyoteTime;

        // Bools
        private bool fishing;
        public bool PressedCatch { get; set; }

        private FishingSystem()
        {
            instance = this;
        }

        private void Start()
        {
            if (fishList == null || fishList.Count <= 0)
                Debug.LogError("FishingSystem fishList is null");
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnPlayerJoined(SO_PlayerCollection _currentPlayers)
        {
            var fishingSystemManager = _currentPlayers.Players[^1].PlayerPrefab.GetComponentInChildren<FishingSystemManager>();

            if (fishingSystemManager != null)
            {
                fishingSystemManager.OnPlayerJoined(_currentPlayers.Players[^1].PlayerScore, _currentPlayers.Players.Count - 1);
                fishingSystemManager.SetUpFishDisplay(fishList);
            }
            else
            {
                Debug.LogError("FishingSystemManager not found");
            }
        }

        public void StartFishing(FishingSystemManager _fishingSystemManager)
        {
            fishing = true;

            _fishingSystemManager.FishingRoutine = StartCoroutine(FishingCoroutine(_fishingSystemManager));
        }

        public void StopFishing(FishingSystemManager _fishingSystemManager)
        {
            EndCoroutine(_fishingSystemManager);
            
            fishing = false;
            _fishingSystemManager.FishHooked = false;
        }

        private SO_Fish CalculateFishProbability()
        {
            int totalProbability = 0;

            for (int i = 0; i < fishList.Count; i++)
            {
                var fish = fishList[i];
                totalProbability += fish.Probability;
            }

            int randomProbability = Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
            int cumulativeProbability = 0;

            for (int j = 0; j < fishList.Count; j++)
            {
                var fish = fishList[j];
                cumulativeProbability += fish.Probability;

                if (randomProbability <= cumulativeProbability)
                    return fish;
            }

            Debug.LogError("ERROR: Cumulative probability doesn't match");
            return null;
        }

        private IEnumerator FishingCoroutine(FishingSystemManager _fishingSystemManager)
        {
            bool caughtAFish = false;
            SO_Fish caughtFish = null;

            // Make it so this repeats if no fish was caught!!
            while (fishing)
            {
                float randomSecondsUntilBite = Random.Range(secondsUntilFishBiteRange.x, secondsUntilFishBiteRange.y);
                yield return new WaitForSeconds(randomSecondsUntilBite);

                caughtFish = CalculateFishProbability();

                _fishingSystemManager.FishHooked = true;
                _fishingSystemManager.PlayFishBitingAnimation(true);

                float randomSecondsToPressCatch = Random.Range(buttonPressTimerRange.x, buttonPressTimerRange.y);
                yield return new WaitForSeconds(randomSecondsToPressCatch);

                if (!PressedCatch)
                    _fishingSystemManager.StopFishBitingAnimation();

                yield return new WaitForSeconds(coyoteTime.Value);

                if (PressedCatch)
                {
                    _fishingSystemManager.PlayFishBitingAnimation(false);
                    fishing = false;

                    _fishingSystemManager.StartFishEvent(caughtFish);
                    yield return new WaitUntil(_fishingSystemManager.CheckIfCatchEventFinished);

                    caughtAFish = _fishingSystemManager.CheckIfCatchEventSucceeded();
                }
                else
                    _fishingSystemManager.DisplayIcon(EEmotion.Embarrassed);

                PressedCatch = false;
                _fishingSystemManager.FishHooked = false;
            }

            if (caughtAFish)
            {
                _fishingSystemManager.DisplayIcon(EEmotion.Love);

                _fishingSystemManager.PullBackFishingRod(false);
                _fishingSystemManager.DisplayFish(caughtFish);

                _fishingSystemManager.FishDisplayActive = true;

                _fishingSystemManager.UpdatePoints(caughtFish.Points);
            }
            else
            {
                _fishingSystemManager.PullBackFishingRod(false);
                _fishingSystemManager.DisplayIcon(EEmotion.Sad);
            }

            EndCoroutine(_fishingSystemManager);
            yield return null;
        }

        private void EndCoroutine(FishingSystemManager _fishingSystemManager)
        {
            if (_fishingSystemManager.FishingRoutine != null)
            {
                _fishingSystemManager.StopFishBitingAnimation();

                StopCoroutine(_fishingSystemManager.FishingRoutine);
                _fishingSystemManager.FishingRoutine = null;
            }
        }
    }
}