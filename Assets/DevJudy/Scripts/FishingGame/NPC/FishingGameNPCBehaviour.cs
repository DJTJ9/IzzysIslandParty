using System.Collections;
using enums;
using Juice;
using ScriptableObjects;
using UnityEngine;

namespace FishingGame.NPCs
{
    public class FishingGameNPCBehaviour : Controller
    {
        private const int numberOfCatchEvents = 3;
        private static readonly int cast = Animator.StringToHash("IsCast");
        private static readonly int fishBiting = Animator.StringToHash("FishBiting");
        
        [SerializeField] private NPCFishingSystem fishingSystem;
        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private Animator animator;
        
        private GameScoreSO gameScore;
        private SO_Fish hookedFish;

        [Header("Random wait times: ")]
        [SerializeField] private Vector2 timeBetweenCasting = new Vector2(0.8f, 2.8f);

        [SerializeField] private Vector2 timeUntilFishBites = new Vector2(3.5f, 5.2f);
        [SerializeField] private Vector2 waitTimeBetweenFishEvents = new Vector2(1.5f, 2.8f);

        [Header("Random chances: ")]
        [SerializeField] private int chanceToHookFish;

        [SerializeField] private int chanceToFinishQTESuccessfully;

        private Coroutine fishingCoroutine;
        private Coroutine waitingToFishCoroutine;

        private bool fishing = false;
        private bool fishFailed = false;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnNPCJoined(GameScoreSO _gameScore)
        {
            gameScore = _gameScore;
        }

        private void StartFishing()
        {
            animator.SetBool(cast, true);

            if (fishingCoroutine != null)
            {
                StopCoroutine(fishingCoroutine);
                fishingCoroutine = null;
            }

            fishing = true;
            fishingCoroutine = StartCoroutine(FishingCoroutine());

            StopCoroutine(waitingToFishCoroutine);
            waitingToFishCoroutine = null;
        }

        private void StopFishing()
        {
            animator.SetBool(cast, false);

            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
            fishing = false;
            
            waitingToFishCoroutine = null;
            waitingToFishCoroutine = StartCoroutine(WaitToFishAgain());
        }

        private IEnumerator FishingCoroutine()
        {
            while (fishing)
            {
                var randomTimeToFishBite = Random.Range(timeUntilFishBites.x, timeUntilFishBites.y);
                yield return new WaitForSecondsRealtime(randomTimeToFishBite);
                
                animator.SetBool(fishBiting, true);
                iconHandler.DisplayIcon(EEmotion.Alert);
                
                yield return new WaitForSecondsRealtime(Random.Range(timeUntilFishBites.x, timeUntilFishBites.y));

                var randomChanceToHookFish = Random.Range(0, 101);

                if (randomChanceToHookFish <= chanceToHookFish)
                {
                    animator.SetBool(fishBiting, false);
                    iconHandler.DisplayIcon(EEmotion.Embarrassed);
                    
                    fishFailed = true;

                    continue;
                }
                
                iconHandler.DisplayIcon(EEmotion.Happy);
                hookedFish = fishingSystem.CalculateFishProbability();
                
                int i = 0;

                while (i < numberOfCatchEvents && !fishFailed)
                {
                    i++;

                    var randomWaitTime = Random.Range(waitTimeBetweenFishEvents.x, waitTimeBetweenFishEvents.y);
                    yield return new WaitForSecondsRealtime(randomWaitTime);

                    var randomChanceToFinishQTESuccessfully = Random.Range(0, 101);

                    if (randomChanceToFinishQTESuccessfully > chanceToFinishQTESuccessfully)
                    {
                        fishFailed = true;
                        iconHandler.DisplayIcon(EEmotion.Angry);
                        
                        StopFishing();
                    }
                }

                if (!fishFailed)
                {
                    iconHandler.DisplayIcon(EEmotion.Love);
                    gameScore.Value += hookedFish.Points;
                    
                    fishing = false;
                }
            }
            StopFishing();

            yield return null;
        }

        private IEnumerator WaitToFishAgain()
        {
            var randomWaitTime = Random.Range(timeBetweenCasting.x, timeBetweenCasting.y);
            yield return new WaitForSecondsRealtime(randomWaitTime);

            StartFishing();
        }
    }
}