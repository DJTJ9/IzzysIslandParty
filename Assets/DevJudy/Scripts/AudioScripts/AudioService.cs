using System;
using System.Collections;
using enums;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Audio
{
    public class AudioService : MonoBehaviour
    {
        private static AudioService instance;
        public static AudioService Instance => instance;

        [SerializeField] private AudioSource audioObject;
        [SerializeField] private AudioMixerGroup mainMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] private AudioMixerGroup sfxMixer;

        // !!TBA object pooling
        // private AudioSource[] pool;

        private AudioService()
        {
            instance = this;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Create and return an audioSource from an audioClip
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volume"></param>
        /// <param name="_pitch"></param>
        /// <returns></returns>
        public AudioSource CreateSound(AudioClip _clip, EAudioType _audioType, float _volume = 1f, float _pitch = 1f)
        {
            AudioSource audioSource = Instantiate(audioObject, parent: this.transform);

            audioSource.clip = _clip;

            audioSource.outputAudioMixerGroup = GetAudioMixerGroup(_audioType);

            audioSource.volume = _volume;
            audioSource.pitch = _pitch;

            return audioSource;
        }

        /// <summary>
        /// Create and return a random audioSource from an array of audioClips
        /// </summary>
        /// <param name="_clips"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volume"></param>
        /// <param name="_pitch"></param>
        /// <returns></returns>
        public AudioSource CreateRandomSound(AudioClip[] _clips, EAudioType _audioType, float _volume = 1f, float _pitch = 1f)
        {
            var random = Random.Range(0, _clips.Length);
            AudioClip clip = _clips[random];

            AudioSource audioSource = CreateSound(clip, _audioType, _volume, _pitch);

            return audioSource;
        }

        private AudioMixerGroup GetAudioMixerGroup(EAudioType _audioType)
        {
            switch (_audioType)
            {
                case EAudioType.Main:
                    return mainMixer;
                case EAudioType.Music:
                    return musicMixer;
                case EAudioType.SFX:
                    return sfxMixer;
                default:
                    Debug.LogError($"Audio Type {_audioType} is not supported.");
                    return null;
            }
        }

        /// <summary>
        /// Play an audioSource once and if handleDeletion is enabled, delete it after it's done playing
        /// Use for audios like ui-sounds
        /// </summary>
        /// <param name="_audioSource"></param>
        /// <param name="_handleDeletion"></param>
        public void PlaySound(AudioSource _audioSource, bool _handleDeletion = true)
        {
            _audioSource.Play();

            if (_handleDeletion)
                StopSoundWhenFinished(_audioSource);
        }

        /// <summary>
        /// Create a random audioSource from the given array, play it once and then delete it. Use only when no reference to the source is needed.
        /// Use for audios with variety like noises from characters
        /// </summary>
        /// <param name="_clips"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volume"></param>
        public void PlayRandomSound(AudioClip[] _clips, EAudioType _audioType, float _volume = 1f)
        {
            AudioSource source = CreateRandomSound(_clips, _audioType, _volume);

            PlaySound(source);
        }


        /// <summary>
        /// Create a random audioSource from the given array in the given volumeRange and pitchRange, play it once and then delete it.
        /// Use for audios like footsteps
        /// Use only when no reference to the source is needed.
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volumeRange"></param>
        /// <param name="_pitchRange"></param>
        public void PlaySoundWithRandomPitch(AudioClip _clip, EAudioType _audioType, Vector2 _volumeRange, Vector2 _pitchRange)
        {
            var randomVolume = Random.Range(_volumeRange.x, _volumeRange.y);
            var randomPitch = Random.Range(_pitchRange.x, _pitchRange.y);

            AudioSource source = CreateSound(_clip, _audioType, randomVolume, randomPitch);

            PlaySound(source);
        }

        /// <summary>
        /// Create a random audioSource from the given array in the given volumeRange and pitchRange, play it once and then delete it.
        /// Use for audios like footsteps
        /// Use only when no reference to the source is needed.
        /// </summary>
        /// <param name="_clips"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volumeRange"></param>
        /// <param name="_pitchRange"></param>
        public void PlayRandomSoundWithRandomPitch(AudioClip[] _clips, EAudioType _audioType, Vector2 _volumeRange, Vector2 _pitchRange)
        {
            var randomVolume = Random.Range(_volumeRange.x, _volumeRange.y);
            var randomPitch = Random.Range(_pitchRange.x, _pitchRange.y);

            AudioSource source = CreateRandomSound(_clips, _audioType, randomVolume, randomPitch);

            PlaySound(source);
        }

        /// <summary>
        /// Play a sound while the given condition is met, then fade it out or delete it immediately
        /// Use for audios like background music
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_clip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volume"></param>
        public void PlaySoundWhile(Func<bool> _condition, AudioClip _clip, EAudioType _audioType, float _volume = 1f)
        {
            StartCoroutine(PlaySoundContinuously(_condition, _clip, _audioType, _volume, false, false));
        }

        /// <summary>
        /// Play a sound while the given condition is met, then fade it out or delete it immediately
        /// Use for audios like background music
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_clip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volume"></param>
        /// <param name="_fadeOut"></param>
        /// <param name="_fadeOutSpeed"></param>
        public void PlaySoundWhile(Func<bool> _condition, AudioClip _clip, EAudioType _audioType,
            bool _fadeOut, float _fadeOutSpeed = 0.5f, float _volume = 1f)
        {
            StartCoroutine(PlaySoundContinuously(_condition, _clip, _audioType, _volume, _fadeOut, false, _fadeOutSpeed));
        }

        /// <summary>
        /// Play a sound while the given condition is met, then fade it out or delete it immediately
        /// Use for audios like background music
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_clip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_goalVolume"></param>
        /// <param name="_fadeOut"></param>
        /// <param name="_fadeIn"></param>
        /// <param name="_fadeSpeed"></param>
        public void PlaySoundWhile(Func<bool> _condition, AudioClip _clip, EAudioType _audioType,
            bool _fadeOut, bool _fadeIn, float _fadeSpeed = 0.5f, float _goalVolume = 1f)
        {
            StartCoroutine(PlaySoundContinuously(_condition, _clip, _audioType, _goalVolume, _fadeOut, _fadeIn, _fadeSpeed));
        }

        /// <summary>
        /// Play a sound until the given condition is met, then fade it out or delete it immediately.
        /// Use for audios like background music
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_audioClip"></param>
        /// <param name="_audioType"></param>
        /// <param name="_goalVolume"></param>
        /// <param name="_fadeOut"></param>
        /// <param name="_fadeIn"></param>
        /// <param name="_fadeSpeed"></param>
        /// <returns></returns>
        private IEnumerator PlaySoundContinuously(Func<bool> _condition, AudioClip _audioClip, EAudioType _audioType, float _goalVolume,
            bool _fadeOut, bool _fadeIn, float _fadeSpeed = 0.2f)
        {
            AudioSource audioSource = CreateSound(_audioClip, _audioType, _goalVolume);

            if (_fadeIn)
                StartCoroutine(FadeInSound(audioSource, _fadeSpeed, _goalVolume));

            while (_condition())
            {
                if (!audioSource.isPlaying)
                    PlaySound(audioSource, false);

                yield return new WaitForFixedUpdate();
            }

            if (_fadeOut)
                StartCoroutine(FadeOutSound(audioSource, _fadeSpeed));
            else
                StopSoundImmediately(audioSource);

            yield return new WaitUntil(() => audioSource == null);
        }

        /// <summary>
        /// Play a random sound with a random volume and pitch while the given condition is met, then delete it immediately.
        /// Use for audios like footsteps while walking
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_audioType"></param>
        /// <param name="_clips"></param>
        /// <param name="_volumeRange"></param>
        /// <param name="_pitchRange"></param>
        /// <returns></returns>
        public void PlayRandomSoundWhile(Func<bool> _condition, AudioClip[] _clips, EAudioType _audioType,
            Vector2 _volumeRange, Vector2 _pitchRange)
        {
            StartCoroutine(PlayRandomSoundContinuously(_condition, _clips, _audioType, _volumeRange, _pitchRange));
        }

        /// <summary>
        /// Play a random sound with a random volume and pitch until the given condition is met, then delete it immediately.
        /// Use for audios like background music
        /// </summary>
        /// <param name="_condition"></param>
        /// <param name="_clips"></param>
        /// <param name="_audioType"></param>
        /// <param name="_volumeRange"></param>
        /// <param name="_pitchRange"></param>
        /// <returns></returns>
        private IEnumerator PlayRandomSoundContinuously(Func<bool> _condition, AudioClip[] _clips, EAudioType _audioType,
            Vector2 _volumeRange, Vector2 _pitchRange)
        {
            AudioSource audioSource = null;

            while (_condition())
            {
                if (audioSource == null || !audioSource.isPlaying)
                {
                    audioSource = CreateRandomSound(_clips, _audioType);

                    PlaySoundWithRandomPitch(audioSource.clip, _audioType, _volumeRange, _pitchRange);
                }

                yield return new WaitForFixedUpdate();
            }

            StopSoundImmediately(audioSource);
        }

        private IEnumerator FadeInSound(AudioSource _audioSource, float _fadeInSpeed, float _goalVolume)
        {
            _audioSource.volume = 0f;
            PlaySound(_audioSource, false);

            while (_audioSource.volume < (_goalVolume - 0.1f))
            {
                _audioSource.volume += (0.2f * _fadeInSpeed);

                yield return new WaitForFixedUpdate();
            }

            yield return null;
        }

        /// <summary>
        /// Waits until the sound is done playing to delete it
        /// </summary>
        /// <param name="_audioSource"></param>
        public void StopSoundWhenFinished(AudioSource _audioSource)
        {
            float clipLength = _audioSource.clip.length;

            Destroy(_audioSource.gameObject, clipLength);
        }

        /// <summary>
        /// Deletes the sound immediately
        /// </summary>
        /// <param name="_audioSource"></param>
        public void StopSoundImmediately(AudioSource _audioSource)
        {
            Destroy(_audioSource.gameObject);
        }

        /// <summary>
        /// Fades the sound out by 0.1f multiplied by the given fadeOutSpeed over each fixedUpdate, then deletes it
        /// </summary>
        /// <param name="_audioSource"></param>
        /// <param name="_fadeOutSpeed"></param>
        public void StopSoundFadeOut(AudioSource _audioSource, float _fadeOutSpeed)
        {
            StartCoroutine(FadeOutSound(_audioSource, _fadeOutSpeed));
        }

        /// <summary>
        /// Fades the sound out by 0.1f multiplied by the given fadeOutSpeed over each fixedUpdate, then deletes it
        /// </summary>
        /// <param name="_audioSource"></param>
        /// <param name="_fadeOutSpeed"></param>
        /// <returns></returns>
        private IEnumerator FadeOutSound(AudioSource _audioSource, float _fadeOutSpeed)
        {
            while (_audioSource.volume > 0.02f)
            {
                _audioSource.volume -= (0.1f * _fadeOutSpeed);

                yield return new WaitForFixedUpdate();
            }

            StopSoundImmediately(_audioSource);
        }
    }
}