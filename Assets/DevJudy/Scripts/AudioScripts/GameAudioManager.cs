using System;
using enums;
using Helper;
using UnityEngine;

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

        private bool isFinished;

        private void Awake()
        {
            levelAudioCollection = GetComponentInChildren<AudioCollection>();
            if (levelAudioCollection == null)
                Debug.LogError("AudioCollection is null");

           // AudioService.Instance.gameObject.SetActive(true);
           // AudioService.Instance.enabled = true;
        }

        public void StartMusic()
        {
            if (audioService == null)
                Debug.LogError("AudioService is null");
            else
                audioService.PlaySoundWhile(() => !isFinished, levelAudioCollection.levelSoundsDictionary.LevelAudios[levelBackgroundMusic],
                    EAudioType.Music, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);

            //StartCoroutine(WaitForLoading(() => !isFinished));
        }

        public void StartBackgroundMusic(Func<bool> _condition)
        {
            if (audioService == null)
                Debug.LogError("AudioService is null");
            else
                audioService.PlaySoundWhile(_condition, levelAudioCollection.levelSoundsDictionary.LevelAudios[levelBackgroundMusic],
                    EAudioType.Music, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);

            //StartCoroutine(WaitForLoading(_condition));
        }

       // private IEnumerator WaitForLoading(Func<bool> _condition)
       // {
       //     yield return new WaitForEndOfFrame();
//
       //     //gameObject.SetActive(true);
       //     AudioService.Instance.gameObject.SetActive(true);
       //     AudioService.Instance.enabled = true;
//
       //     while (!AudioService.Instance.enabled || !AudioService.Instance.gameObject.activeInHierarchy)
       //     {
       //         Debug.Log("AudioService.Instance.go is active: " + AudioService.Instance.gameObject.activeSelf);
       //         Debug.Log("AudioService.Instance.go is activeInHi: " + AudioService.Instance.gameObject.activeInHierarchy);
       //         Debug.Log("AudioService.Instance.enabled is active: " + AudioService.Instance.enabled);
//
       //         gameObject.SetActive(true);
       //         AudioService.Instance.gameObject.SetActive(true);
       //         AudioService.Instance.enabled = true;
//
       //         yield return new WaitForEndOfFrame();
       //     }
//
       //     AudioService.Instance.PlaySoundWhile(_condition, levelAudioCollection.levelSoundsDictionary.LevelAudios[levelBackgroundMusic],
       //         EAudioType.Music, true, true, bgmFadeInSpeed.Value, bgmVolume.Value);
       // }
    }
}