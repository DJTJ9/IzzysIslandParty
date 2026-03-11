using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-1000)]
public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;

    [FoldoutGroup("Volume SOs", expanded: false)] 
    [SerializeField] private SO_FloatVariable masterVolume;
    [SerializeField] private SO_FloatVariable musicVolume;
    [SerializeField] private SO_FloatVariable soundFXVolume;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeAudioVolumes();
    }

    public void SetMasterVolume(float _volume)
    {
        audioMixer.SetFloat("MainVolume", Mathf.Log10(_volume) * 20f);
    }

    public void SetMusicVolume(float _volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20f);
    }

    public void SetSoundFXVolume(float _volume)
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(_volume) * 20f);
    }

    private void InitializeAudioVolumes()
    {
        audioMixer.GetFloat("MainVolume", out var currentMasterVolume);
        {
            var restoredMasterValue = Mathf.Pow(10f, currentMasterVolume / 20f);
            masterVolume.Value = restoredMasterValue;
        }
        audioMixer.GetFloat("MusicVolume", out var currentMusicVolume);
        {
            var restoredMusicValue = Mathf.Pow(10f, currentMusicVolume / 20f);
            musicVolume.Value = restoredMusicValue;
        }
        audioMixer.GetFloat("EffectsVolume", out var currentSoundFXVolume);
        {
            var restoredSoundFXValue = Mathf.Pow(10f, currentSoundFXVolume / 20f);
            soundFXVolume.Value = restoredSoundFXValue;
        }
    }
}