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
    
    // [FoldoutGroup("Slider"), SerializeField]
    private Slider masterVolumeSlider;
    //
    // [FoldoutGroup("Slider"), SerializeField]
    private Slider musicVolumeSlider;
    //
    // [FoldoutGroup("Slider"), SerializeField]
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
    }

    private void Update()
    {
        SetMasterVolume(masterVolumeSlider.value);
        SetMusicVolume(musicVolumeSlider.value);
        SetSoundFXVolume(soundFXVolumeSlider.value);
    }

    public void SetMasterVolume(float volume) 
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20f);
    }
    
    public void SetMusicVolume(float volume) 
    {
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20f);
    }
    
    public void SetSoundFXVolume(float volume) 
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(volume) * 20f);   
    }
}