using System.Collections.Generic;
using Audio;
using FishingGame.QuickTimeEvents;
using MultiuseScripts;
using Player.Collections;
using UnityEngine;

namespace FishingGame
{
    public class FishingGameLevelService : LevelServiceParent
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        [SerializeField] private GameAudioManager gameAudioManager;
        [SerializeField] private LevelTimer levelTimer;
        [SerializeField] private SO_PlayerCollectionFishingGame playerCollection; // !! Do I need both??
        
        [Header("Temp: ")]
        [SerializeField] private List<string> playerNames;
        [SerializeField] private Vector2 npcPointRange;
        
        private void Start()
        {
            if (gameAudioManager == null || levelTimer == null)
                Debug.LogWarning("GameAudioManager or levelTimer is null");
            else
                gameAudioManager.StartBackgroundMusic(() => !levelTimer.TimerFinished);
        }
        
        public override void StartLevel()
        {
            OnLevelStart.Invoke();
        }
        
        //!! REWORK
        public void GetNPCScores()
        {
            playerCollection.Players[0].PlayerScore.Value = currentPlayers.Players[0].PlayerScore.Value;
            
            for (int i = 1; i < playerCollection.Players.Count; i++)
            {
                if (playerCollection.Players[i].PlayerScore.Value > 0)
                    continue;
                
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