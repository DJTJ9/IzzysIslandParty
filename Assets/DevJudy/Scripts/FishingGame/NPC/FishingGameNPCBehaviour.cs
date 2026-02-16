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

        [Header("Dependencies: ")]
        [SerializeField] private NPCFishingSystem fishingSystem;
        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private Animator animator;

        private LineRenderer lineRenderer;
        [SerializeField] private Transform[] rodLineRendererPositions;
        
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
        private bool fishHooked = false;
        private bool fishFailed = false;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                Debug.LogWarning("No lineRenderer attached to " + gameObject.name);

            lineRenderer.enabled = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void OnNPCJoined(GameScoreSO _gameScore)
        {
            gameScore = _gameScore;
        }

        public void StartFishingCycle()
        {
            waitingToFishCoroutine = StartCoroutine(StartFishingCycleCoroutine());
        }

        private IEnumerator StartFishingCycleCoroutine()
        {
            var randomWaitTime = Random.Range(waitTimeBetweenFishEvents.x, waitTimeBetweenFishEvents.y);
            yield return new WaitForSecondsRealtime(randomWaitTime);

            StartFishing();
        }

        private void StartFishing()
        {
            if (fishingCoroutine != null)
            {
                StopCoroutine(fishingCoroutine);
                fishingCoroutine = null;
            }

            fishing = true;
            animator.SetBool(cast, true);
            
            fishingCoroutine = StartCoroutine(FishingCoroutine());

            if (waitingToFishCoroutine != null)
            {
                StopCoroutine(waitingToFishCoroutine);
                waitingToFishCoroutine = null;
            }
        }

        private void StopFishing()
        {
            animator.SetBool(cast, false);
            
            fishing = false;
            fishHooked = false;
            fishFailed = false;

            if (fishingCoroutine != null)
            {
                StopCoroutine(fishingCoroutine);
                fishingCoroutine = null;
            }

            fishing = false;

            waitingToFishCoroutine = null;
            waitingToFishCoroutine = StartCoroutine(WaitToFishAgain());
        }

        private IEnumerator FishingCoroutine()
        {
            while (fishing)
            {
                if (!fishHooked)
                {
                    var randomTimeToFishBite = Random.Range(timeUntilFishBites.x, timeUntilFishBites.y);
                    yield return new WaitForSecondsRealtime(randomTimeToFishBite);

                    animator.SetBool(fishBiting, true);
                    iconHandler.DisplayIcon(EEmotion.Alert);

                    yield return new WaitForSecondsRealtime(Random.Range(timeUntilFishBites.x, timeUntilFishBites.y));

                    var randomChanceToHookFish = Random.Range(0, 101);

                    if (randomChanceToHookFish > chanceToHookFish)
                    {
                        animator.SetBool(fishBiting, false);
                        iconHandler.DisplayIcon(EEmotion.Embarrassed);

                        fishFailed = true;

                        continue;
                    }
                    
                    iconHandler.DisplayIcon(EEmotion.Happy);
                    hookedFish = fishingSystem.CalculateFishProbability();
                    
                    fishHooked = true;
                }

                int i = 0;

                while (i < numberOfCatchEvents && fishHooked)
                {
                    i++;

                    var randomWaitTime = Random.Range(waitTimeBetweenFishEvents.x, waitTimeBetweenFishEvents.y);
                    yield return new WaitForSecondsRealtime(randomWaitTime);

                    var randomChanceToFinishQTESuccessfully = Random.Range(0, 101);

                    if (randomChanceToFinishQTESuccessfully > chanceToFinishQTESuccessfully)
                    {
                        fishFailed = true;
                        fishHooked = false;
                        
                        animator.SetBool(fishBiting, false);
                        iconHandler.DisplayIcon(EEmotion.Sad);

                        StopFishing();
                    }
                }

                if (!fishFailed)
                {
                    animator.SetBool(fishBiting, false);
                    iconHandler.DisplayIcon(EEmotion.Love);
                    
                    gameScore.Value += hookedFish.Points;
                    
                    StopFishing();
                }

                fishing = false;
            }

            yield return null;
        }

        private IEnumerator WaitToFishAgain()
        {
            var randomWaitTime = Random.Range(timeBetweenCasting.x, timeBetweenCasting.y);
            yield return new WaitForSecondsRealtime(randomWaitTime);

            StartFishing();
        }
        
        private void LateUpdate()
        {
            if (lineRenderer == null || rodLineRendererPositions == null || rodLineRendererPositions.Length < 1)
                return;

            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);

            // The second position is either the animationLure, or the physicsLure depending on what is currently active
            if (rodLineRendererPositions[1].gameObject.activeInHierarchy)
                lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
            else if (rodLineRendererPositions[2].gameObject.activeInHierarchy)
                lineRenderer.SetPosition(1, rodLineRendererPositions[2].position);
        }
    }
}