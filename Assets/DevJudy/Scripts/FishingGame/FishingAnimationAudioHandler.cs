using Audio;
using enums;
using UnityEngine;

namespace FishingGame.Animations
{
    public class FishingAnimationAudioHandler : MonoBehaviour
    {
        [SerializeField] private float lureSplashVolume = 0.6f;
        [SerializeField] private Vector2 fishBitingVolumeRange;
        [SerializeField] private Vector2 fishBitingPitchRange;

        private bool fishBiting = false;
        
        public void PlayLureSplashSound()
        {
            AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["LureSplashing"], EAudioType.SFX, lureSplashVolume);
        }

        public void PlayFishBitingSound()
        {
            AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["FishBiting"], EAudioType.SFX, lureSplashVolume);
        }

        public void StartPlayingFishBitingSound()
        {
            fishBiting = true;
            
            Debug.Log("Playing FishBiting");
           //AudioClip[] fishBitingClips = new AudioClip[]{AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["FishBiting"], 
           //    AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["LureSplashing"]};
           //
           //AudioService.Instance.PlayRandomSoundWhile(()=> fishBiting, fishBitingClips, EAudioType.SFX, fishBitingVolumeRange, fishBitingPitchRange);
        }

        public void StopFishBitingSound()
        {
            Debug.Log("Stopping FishBiting");
            fishBiting = false;
        }
    }
}
