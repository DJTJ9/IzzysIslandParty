using Audio;
using enums;
using MultiuseScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingGameLevelService : LevelServiceParent
    {
        [SerializeField] private SO_PlayerCollection currentPlayers;
        [SerializeField] private GameAudioManager gameAudioManager;
        [SerializeField] private LevelTimer levelTimer;
        
        private bool levelOver = false;
        
        private void Start()
        {
            if (gameAudioManager == null || levelTimer == null)
                Debug.LogWarning("GameAudioManager or levelTimer is null");
            else
            {
                gameAudioManager.StartBackgroundMusic(() => !levelTimer.TimerFinished);
                
                AudioService.Instance.PlaySoundWhile(() => !levelOver, AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["Waves"],
                    EAudioType.SFX, false, false, 1f, 0.7f);
            }
        }
        
        public override void StartLevel()
        {
            OnLevelStart.Invoke();
        }
        
        public override void EndLevel()
        {
            levelOver = true;
            OnLevelEnd.Invoke();
        }
    }
}