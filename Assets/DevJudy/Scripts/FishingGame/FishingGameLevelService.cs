using System.Collections.Generic;
using Audio;
using MultiuseScripts;
using Player.Collections;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingGameLevelService : LevelServiceParent
    {
        [SerializeField] private GameAudioManager gameAudioManager;
        [SerializeField] private LevelTimer levelTimer;

        [SerializeField] private SO_PlayerCollectionFishingGame playerCollection;
        
        [Header("Temp: ")]
        [SerializeField] private UIPointsService playerPoints;
        [SerializeField] private List<string> playerNames;
        [SerializeField] private Vector2 npcPointRange;
        
        private void Start()
        {
            if (gameAudioManager == null || levelTimer == null)
                Debug.LogWarning("GameAudioManager or levelTimer is null");
            else
                gameAudioManager.StartBackgroundMusic(() => !levelTimer.TimerFinished);
            
            StartLevel();
        }

        public override void StartLevel()
        {
            OnLevelStart.Invoke();
        }

        public void GetNPCScores()
        {
            playerCollection.Players[0].PlayerScore.Value = playerPoints.CurrentScore;
            
            for (int i = 1; i < playerCollection.Players.Count; i++)
            {
                float randomScore = Random.Range(npcPointRange.x, npcPointRange.y + 1);
                
                playerCollection.Players[i].PlayerScore.Value = (int)randomScore;

                playerCollection.Players[i].Name = playerNames[i - 1];
            }
        }

        public override void EndLevel()
        {
            Debug.Log("Ending level");
            OnLevelEnd.Invoke();
        }
    }
}