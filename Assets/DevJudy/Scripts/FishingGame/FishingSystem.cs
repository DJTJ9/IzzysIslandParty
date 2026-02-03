using System.Collections;
using System.Collections.Generic;
using enums;
using Juice;
using ScriptableObjects;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    [DefaultExecutionOrder(-100)]
    public class FishingSystem : MonoBehaviour
    {
        [Header("Dependencies: ")]
        // !! Can't have only one controller for multiplayer
        [SerializeField] private FishingRodController fishingRodController;
        [SerializeField] private CatchEventHandler catchEventHandler;
        [SerializeField] private UIPointsService uiPointsService;
        [SerializeField] private FishDisplay fishDisplay;

        [Header("Variables: ")]
        private static FishingSystem instance;

        public static FishingSystem Instance => instance;

        [SerializeField] private List<SO_Fish> fishList;
        [SerializeField] private Vector2 secondsUntilFishBiteRange;
        [SerializeField] private Vector2 buttonPressTimerRange;
        [SerializeField] private FloatReference coyoteTime;

        // Bools
        private bool fishing;
        private bool fishDisplayActive;
        public bool PressedCatch { get; set; }
        public bool FishHooked { get; private set; }

        private FishingSystem()
        {
            instance = this;
        }

        private void Start()
        {
            if (fishingRodController == null)
                Debug.LogError("No FishingRodController assigned");

            if (catchEventHandler == null)
                Debug.LogError("No catchEventHandler assigned");

            if (fishList == null || fishList.Count <= 0)
                Debug.LogError("FishingSystem fishList is null");
            else
            {
                if (fishDisplay != null)
                    SpawnInFishDisplayObjects();
            }

            StopFishDisplay();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void SpawnInFishDisplayObjects()
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                fishList[i].PrefabReference = fishDisplay?.SpawnInFishPrefabs(fishList[i]);
            }
        }

        public void StartFishing()
        {
            fishing = true;

            // !! Cant do the usual coroutine stuff bc this is a singleton...
            fishingRodController.FishingRoutine = StartCoroutine(FishingCoroutine());
        }

        public void StopFishing()
        {
            if (fishingRodController.FishingRoutine != null)
            {
                fishingRodController.StopFishBitingAnimation();

                StopCoroutine(fishingRodController.FishingRoutine);
                fishingRodController.FishingRoutine = null;
            }

            fishing = false;
            FishHooked = false;
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

        private IEnumerator FishingCoroutine()
        {
            bool caughtAFish = false;
            SO_Fish caughtFish = null;

            // Make it so this repeats if no fish was caught!!
            while (fishing)
            {
                float randomSecondsUntilBite = Random.Range(secondsUntilFishBiteRange.x, secondsUntilFishBiteRange.y);
                yield return new WaitForSeconds(randomSecondsUntilBite);

                caughtFish = CalculateFishProbability();

                FishHooked = true;
                fishingRodController.PlayFishBitingAnimation(true);

                float randomSecondsToPressCatch = Random.Range(buttonPressTimerRange.x, buttonPressTimerRange.y);
                yield return new WaitForSeconds(randomSecondsToPressCatch);

                // Stop the animation a few milliseconds before checking input for coyote time

                if (!PressedCatch)
                    fishingRodController.StopFishBitingAnimation();

                yield return new WaitForSeconds(coyoteTime.Value);

                if (PressedCatch)
                {
                    fishingRodController.PlayFishBitingAnimation(false);
                    fishing = false;

                    catchEventHandler.StartFishEvent(caughtFish);
                    yield return new WaitUntil(() => catchEventHandler.CatchEventFinished);

                    caughtAFish = catchEventHandler.CatchEventSuccess;
                }
                else
                    fishingRodController.IconHandler?.DisplayIcon(EEmotion.Embarrassed);

                PressedCatch = false;
                FishHooked = false;
            }

            if (caughtAFish)
            {
                fishingRodController.IconHandler?.DisplayIcon(EEmotion.Love);

                fishDisplay?.DisplayFish(caughtFish);
                fishDisplayActive = true;

                uiPointsService?.UpdatePointsText(caughtFish.Points);

                yield return new WaitForSeconds(3f);

                StopFishDisplay();
            }
            else
                fishingRodController.IconHandler?.DisplayIcon(EEmotion.Sad);

            fishingRodController.PullBackFishingRod();

            yield return null;
        }

        public void StopFishDisplay()
        {
            if (!fishDisplayActive)
                return;

            fishDisplay?.StopDisplayFish();

            fishDisplayActive = false;
        }
    }
}