using System;
using enums;
using UnityEngine;

namespace JetskiGame.Audio
{
   public class JetskiGameAudioManager : MonoBehaviour
   {
      private AudioCollection levelAudioCollection;

      private void Awake()
      {
         levelAudioCollection = GetComponentInChildren<AudioCollection>();
         if (levelAudioCollection == null)
            Debug.LogError("AudioCollection is null");
         
      }

      public void StartBackgroundMusic(Func<bool> _condition)
      {
         Debug.Log("StartBackgroundMusic");
         AudioService.Instance.PlaySoundWhile(_condition, levelAudioCollection.levelSoundsDictionary.LevelAudios["JetskiGameMusic"],
            EAudioType.Music, true, true, 0.2f);
      }
   }
}
