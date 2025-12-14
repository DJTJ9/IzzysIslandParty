using System.Collections;
using System.Collections.Generic;
using UIScripts;
using UnityEngine;
using UnityEngine.Events;

namespace FishingGame{
    public class FishingSystem : MonoBehaviour
    {
        [Header("Dependencies: ")]
        [SerializeField] private UITextManager textManager;
        [SerializeField] private FishingRodController fishingRodController;

        [Header("Variables: ")]
        private static FishingSystem instance;

        [SerializeField] private UnityEvent onFishCaughtEvent;

        [SerializeField] private List<So_Fish> fishList;
        [SerializeField] private Vector2 secondsUntilFishBiteRange;
        [SerializeField] private Vector2 buttonPressTimerRange;
        [SerializeField] private FloatReference coyoteTime;

        // Bools
        public bool PressedCatch { get; set; }
        private bool fishing;
        public bool FishHooked { get; private set; }

        private Coroutine fishingRoutine;

        // To be deleted, only for test!!!
        [SerializeField] private CatchEventHandler catchEventHandler;

        private FishingSystem()
        {
            instance = this;
        }

        public static FishingSystem GetInstance()
        {
            if (instance != null)
                return instance;

            Debug.LogError("FishingSystem instance is null");
            return null;
        }

        private void Start()
        {
            onFishCaughtEvent.AddListener(catchEventHandler.ChooseRandomEvent);
        }
        
        public void StartFishing()
        {
            fishing = true;

            if (fishList == null || fishList.Count <= 0)
            {
                Debug.LogError("FishingSystem fishList is null");
                return;
            }

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

        private So_Fish CalculateFishProbability()
        {
            int totalProbability = 0;

            foreach (So_Fish fish in fishList)
            {
                totalProbability += fish.Probability;
            }

            int randomProbability = Random.Range(0, Mathf.FloorToInt(totalProbability) + 1);
            int cumulativeProbability = 0;

            foreach (So_Fish fish in fishList)
            {
                cumulativeProbability += fish.Probability;

                if (randomProbability <= cumulativeProbability)
                    return fish;
            }

            Debug.LogError("Cumulative probability doesn't match");
            return null;
        }

        private IEnumerator FishingCoroutine()
        {
            bool caughtAFish = false;
            So_Fish caughtFish = null;

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
                    Debug.Log("Pressed Catch");
                    
                    fishing = false;

                    // onFishCaughtEvent.Invoke();
                   catchEventHandler.ChooseRandomEvent(); 

                    yield return new WaitUntil(() => catchEventHandler.CatchEventFinished);

                    caughtAFish = catchEventHandler.CatchEventSuccess;
                }
                else
                {
                    Debug.Log("FS says Catch event over");
                }

                PressedCatch = false;
                FishHooked = false;
            }

            if (caughtAFish)
            {
                // Show caught fish (via Text and/or Picture)!!

                textManager.UpdatePointsText(caughtFish.Points);
                Debug.Log("Congrats!! You caught a " + caughtFish.FishName);

                fishingRodController.PullBackFishingRod();
            }

            StopFishing();

            yield return null;
        }
    }
}
