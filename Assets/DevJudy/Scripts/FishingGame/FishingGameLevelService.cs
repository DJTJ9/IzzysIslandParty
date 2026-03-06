using Audio;
using MultiuseScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingGameLevelService : LevelServiceParent
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        [SerializeField] private GameAudioManager gameAudioManager;
        [SerializeField] private LevelTimer levelTimer;
        
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
        
        public override void EndLevel()
        {
            OnLevelEnd.Invoke();
        }
    }
}