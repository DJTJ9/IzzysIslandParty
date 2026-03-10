using System.Collections;
using System.Collections.Generic;
using enums;
using Helper;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FishingGame
{
    [DefaultExecutionOrder(-100)]
    public class FishingSystem : MonoBehaviour
    {
        private const int maxNumberOfPlayers = 4;
        [Header("Variables: ")]
        private static FishingSystem instance;

        public static FishingSystem Instance => instance;

        [SerializeField] private List<SO_Fish> fishList;
        [SerializeField] private Vector2 secondsUntilFishBiteRange;
        [SerializeField] private Vector2 buttonPressTimerRange;
        [SerializeField] private FloatReference coyoteTime;

        private List<bool> fishing;
        public List<bool> PressedCatch;

        private void Awake()
        {
            if (Instance == null)
                instance = this;
            else
            {
                Debug.LogError("More than one instance of this script exists, destroying self");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (fishList == null || fishList.Count <= 0)
                Debug.LogError("FishingSystem fishList is null");

            fishing = new List<bool>();
            PressedCatch = new List<bool>();
            
            for (int i = 0; i < maxNumberOfPlayers; i++)
            {
                fishing.Add(false);
                PressedCatch.Add(false);
            }

            this.gameObject.SetActive(true);
            this.gameObject.transform.parent.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnPlayerJoined(SO_PlayerCollection _currentPlayers)
        {
            var fishingSystemManager = _currentPlayers.Players[^1].PlayerReference.GetComponentInChildren<FishingSystemManager>();

            if (fishingSystemManager != null)
            {
                fishingSystemManager.OnPlayerJoined(_currentPlayers.Players[^1].PlayerScore, _currentPlayers.Players.Count - 1);
                fishingSystemManager.SetUpFishDisplay(fishList);
            }
        }

        public void StartFishing(FishingSystemManager _fishingSystemManager)
        {
            fishing[_fishingSystemManager.PlayerIndex] = true;
            _fishingSystemManager.FishingRoutine = StartCoroutine(FishingCoroutine(_fishingSystemManager));
        }

        public void StopFishing(FishingSystemManager _fishingSystemManager)
        {
            EndCoroutine(_fishingSystemManager);

            fishing[_fishingSystemManager.PlayerIndex] = false;
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
            while (fishing[_fishingSystemManager.PlayerIndex])
            {
                float randomSecondsUntilBite = Random.Range(secondsUntilFishBiteRange.x, secondsUntilFishBiteRange.y);
                yield return new WaitForSeconds(randomSecondsUntilBite);

                caughtFish = CalculateFishProbability();

                _fishingSystemManager.FishHooked = true;
                _fishingSystemManager.PlayFishBitingAnimation(true);

                float randomSecondsToPressCatch = Random.Range(buttonPressTimerRange.x, buttonPressTimerRange.y);
                yield return new WaitForSeconds(randomSecondsToPressCatch);
                
                if (!PressedCatch[_fishingSystemManager.PlayerIndex])
                    _fishingSystemManager.StopFishBitingAnimation();

                yield return new WaitForSeconds(coyoteTime.Value);

                if (PressedCatch[_fishingSystemManager.PlayerIndex])
                {
                    _fishingSystemManager.PlayFishBitingAnimation(false);
                    fishing[_fishingSystemManager.PlayerIndex] = false;

                    _fishingSystemManager.StartFishEvent(caughtFish);
                    yield return new WaitUntil(_fishingSystemManager.CheckIfCatchEventFinished);

                    caughtAFish = _fishingSystemManager.CheckIfCatchEventSucceeded();
                }
                else
                    _fishingSystemManager.DisplayIcon(EEmotion.Embarrassed);

                PressedCatch[_fishingSystemManager.PlayerIndex] = false;
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