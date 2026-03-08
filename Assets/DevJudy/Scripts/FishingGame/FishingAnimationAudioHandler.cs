using Audio;
using enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FishingGame.Animations
{
    public class FishingAnimationAudioHandler : MonoBehaviour
    {
        [SerializeField] private bool isNPC;
        
        [SerializeField] private float lureSplashVolume = 0.6f;
        [HideIf("isNPC")]
        [SerializeField] private Vector2 fishBitingVolumeRange;
        [HideIf("isNPC")]
        [SerializeField] private Vector2 fishBitingPitchRange;

        private bool fishBiting = false;

        public void PlayLureSplashSound()
        {
            AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["LureSplashing"], EAudioType.SFX,
                lureSplashVolume);
        }

        public void PlayFishBitingSound()
        {
            if (isNPC)
                return;
            
            AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["FishBiting"], EAudioType.SFX,
                lureSplashVolume);
        }

        public void StartPlayingFishBitingSound()
        {
            if (fishBiting || isNPC)
                return;
            
            fishBiting = true;

            Debug.Log("Playing FishBiting");
            AudioClip[] fishBitingClips = new AudioClip[]
            {
                AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["FishBiting"],
            };

            AudioService.Instance.PlayRandomSoundWhile(() => fishBiting, fishBitingClips, EAudioType.SFX, fishBitingVolumeRange,
                fishBitingPitchRange);
        }

        public void StopFishBitingSound()
        {
            if (!fishBiting || isNPC)
                return;
            
            Debug.Log("Stopping FishBiting");
            fishBiting = false;
        }
    }
}