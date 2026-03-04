using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;

    [SerializeField] 
    private AudioMixer audioMixer;

    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider soundFXVolumeSlider;

    private void Awake() 
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        masterVolumeSlider = root.Q<Slider>("settings-master-volume__slider");
        musicVolumeSlider = root.Q<Slider>("settings-music-volume__slider");
        soundFXVolumeSlider = root.Q<Slider>("settings-soundfx-volume__slider");

        masterVolumeSlider.RegisterValueChangedCallback(evt => SetMasterVolume(evt.newValue));
        musicVolumeSlider.RegisterValueChangedCallback(evt => SetMusicVolume(evt.newValue));
        soundFXVolumeSlider.RegisterValueChangedCallback(evt => SetSoundFXVolume(evt.newValue));
    }

    private void SetMasterVolume(float volume) 
    {
        audioMixer.SetFloat("MainVolume", Mathf.Log10(volume) * 20f);
    }

    private void SetMusicVolume(float volume) 
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }

    private void SetSoundFXVolume(float volume) 
    {
        audioMixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20f);
    }
}