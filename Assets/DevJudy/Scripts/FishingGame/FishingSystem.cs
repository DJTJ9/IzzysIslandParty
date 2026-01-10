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
        [SerializeField] private UITextManager textManager;
        [SerializeField] private FishingRodController fishingRodController;

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

        [Header("Temp")]
        [SerializeField] private CatchEventHandler catchEventHandler;

        private FishingSystem()
        {
            instance = this;
        }

        private void Start()
        {
            if (fishList == null || fishList.Count <= 0)
                Debug.LogError("FishingSystem fishList is null");
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

            foreach (SO_Fish fish in fishList)
            {
                totalProbability += fish.Probability;
            }

            int randomProbability = Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
            int cumulativeProbability = 0;

            foreach (SO_Fish fish in fishList)
            {
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
                {
                    IconHandler.Instance.DisplayIcon(EEmotion.Sad);
                }

                PressedCatch = false;
                FishHooked = false;
            }

            if (caughtAFish)
            {
                IconHandler.Instance.DisplayIcon(EEmotion.Love);
                
                // Show caught fish (via Text and/or Picture)!!

                textManager.UpdatePointsText(caughtFish.Points);
                Debug.Log("Congrats!! You caught a " + caughtFish.FishName);

                fishingRodController.PullBackFishingRod();
            }
            else
            {
                IconHandler.Instance.DisplayIcon(EEmotion.Sad);
            }

            StopFishing();

            yield return null;
        }
    }
}