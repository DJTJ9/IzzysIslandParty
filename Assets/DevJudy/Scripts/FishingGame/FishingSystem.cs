using System.Collections;
using System.Collections.Generic;
using enums;
using Juice;
using ScriptableObjects;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingSystem : MonoBehaviour
    {
        [Header("Dependencies: ")]
        [SerializeField] private FishingRodController fishingRodController;
        [SerializeField] private CatchEventHandler catchEventHandler;
        [SerializeField] private UITextManager textManager;
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
        public bool PressedCatch { get; set; }
        public bool FishHooked { get; private set; }

        private Coroutine fishingRoutine;
        
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
        }

        private void SpawnInFishDisplayObjects()
        {
            for (int i = 0; i < fishList.Count; i++)
            {
                var fish = fishList[i];
                fish.PrefabReference = fishDisplay?.SpawnInFishPrefabs(fish);
            }
        }

        public void StartFishing()
        {
            fishing = true;

            if (fishingRoutine == null)
                fishingRoutine = StartCoroutine(FishingCoroutine());
        }

        public void StopFishing()
        {
            if (fishingRoutine != null)
            {
                fishingRodController.StopFishBitingAnimation();

                StopCoroutine(fishingRoutine);
                fishingRoutine = null;
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
                fishingRodController.PlayFishBitingAnimation();

                float randomSecondsToPressCatch = Random.Range(buttonPressTimerRange.x, buttonPressTimerRange.y);
                yield return new WaitForSeconds(randomSecondsToPressCatch);

                // Stop the animation a few milliseconds before checking input for coyote time
                fishingRodController.StopFishBitingAnimation();
                yield return new WaitForSeconds(coyoteTime.Value);

                if (PressedCatch)
                {
                    fishing = false;

                    catchEventHandler.StartFishEvent(caughtFish);
                    yield return new WaitUntil(() => catchEventHandler.CatchEventFinished);

                    caughtAFish = catchEventHandler.CatchEventSuccess;
                }
                else
                    IconHandler.Instance.DisplayIcon(EEmotion.Embarrassed);

                PressedCatch = false;
                FishHooked = false;
            }

            if (caughtAFish)
            {
                IconHandler.Instance.DisplayIcon(EEmotion.Love);
                
                fishDisplay?.DisplayFish(caughtFish);
                textManager?.UpdatePointsText(caughtFish.Points);

                // TBD and replaced with a button to exit the display!!
                yield return new WaitForSeconds(3f);
                fishDisplay?.StopDisplayFish();
                
                fishingRodController.PullBackFishingRod();
            }
            else
                IconHandler.Instance.DisplayIcon(EEmotion.Sad);

            StopFishing();

            yield return null;
        }
    }
}