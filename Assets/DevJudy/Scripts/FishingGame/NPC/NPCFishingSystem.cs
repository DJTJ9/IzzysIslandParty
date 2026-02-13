using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace FishingGame.NPCs
{
    public class NPCFishingSystem : MonoBehaviour
    {
        [SerializeField] private List<SO_Fish> fishList;

        public SO_Fish CalculateFishProbability()
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
    }
}
