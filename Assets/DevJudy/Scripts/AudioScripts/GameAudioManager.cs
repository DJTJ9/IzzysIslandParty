using System;
using enums;
using Helper;
using UnityEngine;
using UnityEngine.Events;

namespace Audio
{
    [DefaultExecutionOrder(-3000)]
    public class GameAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioService audioService;
        private AudioCollection levelAudioCollection;
        [SerializeField] private FloatReference bgmVolume;
        [SerializeField] private FloatReference bgmFadeInSpeed;

        // !! TEMP, to be changed to an enum/SO
        [Header("TEMP: ")]
        [SerializeField] private string levelBackgroundMusic;
        [SerializeField] private UnityEvent onMusicEnd;

        private bool isRunning;
        private bool isFinished;

        private void Awake()
        {
            levelAudioCollection = GetComponentInChildren<AudioCollection>();
            
            if (levelAudioCollection == null)
                Debug.LogError("AudioCollection is null");
        }

        public void StartMusic()
        {
            if (isRunning)
                return;
            
            if (audioService == null)
                Debug.LogError("AudioService is null");
            else
            {
                audioService.PlaySoundWhile(() => !isFinished, levelAudioCollection.LevelSoundsDictionary.LevelAudios[levelBackgroundMusic],
                    EAudioType.Music, onMusicEnd, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);
                
                isRunning = true;
            }
        }

        public void StartBackgroundMusic(Func<bool> _condition)
        {
            if (isRunning)
                return;
            
            if (audioService == null)
                Debug.LogError("AudioService is null");
            else
            {
                audioService.PlaySoundWhile(_condition, levelAudioCollection.LevelSoundsDictionary.LevelAudios[levelBackgroundMusic],
                    EAudioType.Music, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);
                
                isRunning = true;
            }
            
        }

        public void SetIsRunningFalse()
        {
            isRunning = false;
        }
    }
}