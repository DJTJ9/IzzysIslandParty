using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioService : MonoBehaviour
{
    private static AudioService instance;
    public static AudioService Instance => instance;

    [SerializeField] private AudioSource audioObject;

    [SerializeField] private AudioClip testClip;

    private AudioService()
    {
        instance = this;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        AudioSource source = CreateSound(testClip, transform);
        PlaySound(source);
    }

    // !!TBA object pooling
    
    // !!TBA choosing which mixer to play from
    
    // TBD think about all the possible sound effects we need, and how they might have to be implemented (f.ex. music, fishStruggling-noise, jetski noise,
    // the noise of the ball rolling, etc), how they might want to be interacted with, and what use case would be most ideal for those scripts

    /// <summary>
    /// Create and return an audioSource from an audioClip
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volume"></param>
    /// <param name="_pitch"></param>
    /// <returns></returns>
    public AudioSource CreateSound(AudioClip _clip, Transform _spawnTransform, float _volume = 1f, float _pitch = 1f)
    {
        AudioSource audioSource = Instantiate(audioObject, _spawnTransform);

        audioSource.clip = _clip;
        audioSource.volume = _volume;
        audioSource.pitch = _pitch;
        //audioSource.outputAudioMixerGroup

        return audioSource;
    }

    /// <summary>
    /// Create and return a random audioSource from an array of audioClips
    /// </summary>
    /// <param name="_clips"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volume"></param>
    /// <param name="_pitch"></param>
    /// <returns></returns>
    public AudioSource CreateRandomSound(AudioClip[] _clips, Transform _spawnTransform, float _volume = 1f, float _pitch = 1f)
    {
        var random = Random.Range(0, _clips.Length);
        AudioClip clip = _clips[random];
        
        AudioSource audioSource = CreateSound(clip, _spawnTransform, _volume, _pitch);

        return audioSource;
    }

    /// <summary>
    /// Play an audioSource once
    /// </summary>
    /// <param name="_audioSource"></param>
    /// <param name="_deleteSourceWhenFinished"></param>
    public void PlaySound(AudioSource _audioSource, bool _deleteSourceWhenFinished = true)
    {
        _audioSource.Play();

        if (_deleteSourceWhenFinished)
            StopSoundWhenFinished(_audioSource);
    }

    /// <summary>
    /// Create a random audioSource from the given array, play it once and then delete it. Used when no reference to the source is needed.
    /// </summary>
    /// <param name="_clips"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volume"></param>
    public void PlayRandomSound(AudioClip[] _clips, Transform _spawnTransform, float _volume = 1f)
    {
        AudioSource source = CreateRandomSound(_clips, _spawnTransform, _volume);

        PlaySound(source);
    }

    /// <summary>
    /// Create a random audioSource from the given array in the given volumeRange and pitchRange, play it once and then delete it.
    /// Used when no reference to the source is needed.
    /// </summary>
    /// <param name="_clips"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volumeRange"></param>
    /// <param name="_pitchRange"></param>
    public void PlayRandomSoundWithRandomPitch(AudioClip[] _clips, Transform _spawnTransform, Vector2 _volumeRange, Vector2 _pitchRange)
    {
        var randomVolume = Random.Range(_volumeRange.x, _volumeRange.y);
        var randomPitch = Random.Range(_pitchRange.x, _pitchRange.y);
        
        AudioSource source = CreateRandomSound(_clips, _spawnTransform, randomVolume, randomPitch);
        
        PlaySound(source);
    }

    /// <summary>
    /// Start the PlaySoundContinuously Coroutine for the given audioClip
    /// </summary>
    /// <param name="_condition"></param>
    /// <param name="_clip"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volume"></param>
    public void PlaySoundUntil(Func<bool> _condition, AudioClip _clip, Transform _spawnTransform, float _volume = 1f)
    {
        StartCoroutine(PlaySoundContinuously(_condition, _clip, _spawnTransform, _volume));
    }

    // TBA !! Play reandom sounds with random pitch (f.ex footsteps)
    public void PlayRandomSoundUntil(Func<bool> _condition, AudioClip[] _clips, Transform _spawnTransform, float _volume = 1f)
    {
    }

    /// <summary>
    /// Play a sound until the given condition is met, then fade it out or delete it immediately.
    /// </summary>
    /// <param name="_condition"></param>
    /// <param name="_audioClip"></param>
    /// <param name="_spawnTransform"></param>
    /// <param name="_volume"></param>
    /// <param name="_fadeOut"></param>
    /// <param name="_fadeOutSpeed"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundContinuously(Func<bool> _condition, AudioClip _audioClip, Transform _spawnTransform, float _volume,
        bool _fadeOut = true, float _fadeOutSpeed = 1f)
    {
        AudioSource audioSource = CreateSound(_audioClip, _spawnTransform, _volume);

        while (!_condition())
        {
            PlaySound(audioSource, !_fadeOut);

            yield return new WaitForSeconds(_audioClip.length);
        }

        if (_fadeOut)
            StartCoroutine(FadeOutSound(audioSource, _fadeOutSpeed));

        yield return new WaitUntil(() => audioSource == null);
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

    /// <summary>
    /// Waits until the sound is done playing to delete it
    /// </summary>
    /// <param name="_audioSource"></param>
    private void StopSoundWhenFinished(AudioSource _audioSource)
    {
        float clipLength = _audioSource.clip.length;

        Destroy(_audioSource.gameObject, clipLength);
    }

    /// <summary>
    /// Deletes the sound immediately
    /// </summary>
    /// <param name="_audioSource"></param>
    private void StopSoundImmediately(AudioSource _audioSource)
    {
        Destroy(_audioSource.gameObject);
    }
}