using System;
using enums;
using UnityEngine;

namespace Audio
{
    public class GameAudioManager : MonoBehaviour
    {
        private AudioCollection levelAudioCollection;
        [SerializeField] private FloatReference bgmVolume;
        [SerializeField] private FloatReference bgmFadeInSpeed;

        // !! TEMP, to be changed to an enum/SO
        [Header("TEMP: ")]
        [SerializeField] private string levelBackgroundMusic;

        private bool isFinished;
        
        private void Awake()
        {
            levelAudioCollection = GetComponentInChildren<AudioCollection>();
            if (levelAudioCollection == null)
                Debug.LogError("AudioCollection is null");
        }

        public void StartMusic()
        {
            StartBackgroundMusic(() => !isFinished);
        }
        
        public void StartBackgroundMusic(Func<bool> _condition)
        {
            AudioService.Instance.PlaySoundWhile(_condition, levelAudioCollection.levelSoundsDictionary.LevelAudios[levelBackgroundMusic],
                EAudioType.Music, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);
        }
    }
}