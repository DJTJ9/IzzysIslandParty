using Audio;
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
        
        [Header("Temp: ")]
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
        
        //!! Put in NPC script
        public void GetNPCScores()
        {
            
            for (int i = 1; i < currentPlayers.Players.Count; i++)
            {
                if (currentPlayers.Players[i].PlayerScore.Value > 0)
                    continue;
                
                float randomScore = Random.Range(npcPointRange.x, npcPointRange.y + 1);
                
                currentPlayers.Players[i].PlayerScore.Value = (int)randomScore;
            }
        }

        public override void EndLevel()
        {
            OnLevelEnd.Invoke();
        }
    }
}