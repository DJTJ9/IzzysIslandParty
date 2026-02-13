using System.Collections;
using UnityEngine;

namespace FishingGame.NPCs
{
    public class FishingGameNPCBehaviour : Controller
    {
        private const int numberOfCatchEvents = 3;

        [Header("Random wait times: ")]
        [SerializeField] private Vector2 timeBetweenCasting = new Vector2(0.8f, 2.8f);
        [SerializeField] private Vector2 timeUntilFishBites = new Vector2(3.5f, 5.2f);
        [SerializeField] private Vector2 waitTimeBetweenFishEvents = new Vector2(1.5f, 2.8f);
        
        [Header("Random chances: ")]
        [SerializeField] private Vector2 chanceToHookFish;
        [SerializeField] private Vector2 chanceToFinishQTESuccessfully;

        private Coroutine fishingCoroutine;
        private Coroutine waitingToFishCoroutine;

        private bool fishFailed = false;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void StartFishing()
        {
            // Play casting animation 
            
            if (fishingCoroutine != null)
            {
                StopCoroutine(fishingCoroutine);
                fishingCoroutine = null;
            }
            fishingCoroutine = StartCoroutine(FishingCoroutine());
            
            StopCoroutine(waitingToFishCoroutine);
            waitingToFishCoroutine = null;
        }

        private IEnumerator FishingCoroutine()
        {
            var randomTimeToFishBite = Random.Range(timeUntilFishBites.x, timeUntilFishBites.y);
            yield return new WaitForSecondsRealtime(randomTimeToFishBite);

            // Small chance to fail the hooked fish
            // If fish caught check which one it should be

            int i = 0;

            while (i < numberOfCatchEvents && !fishFailed)
            {
                i++;

                var randomWaitTime = Random.Range(waitTimeBetweenFishEvents.x, waitTimeBetweenFishEvents.y);
                yield return new WaitForSecondsRealtime(randomWaitTime);

                // Check if the event should succeed
                // if failed, fishFailed = true;
            }

            // Check again if fish was failed
            // Play the according animation/icon
            // Add to points if necessary

            waitingToFishCoroutine = StartCoroutine(WaitToFishAgain());

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