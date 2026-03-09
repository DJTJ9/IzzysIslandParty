using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-1000)]
public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;

    [SerializeField] 
    private AudioMixer audioMixer;

    private void Awake() 
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Destroy(gameObject);
        }
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
}