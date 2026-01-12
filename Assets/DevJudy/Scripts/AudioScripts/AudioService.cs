using UnityEngine;

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

    private void Start()
    {
        PlaySound(testClip, transform);
    }
    
    // !!TBA object pooling
    public void PlaySound(AudioClip _clip, Transform _spawnTransform, float _volume = 1f)
    {
        AudioSource audioSource = Instantiate(audioObject, _spawnTransform);
        
        audioSource.clip = _clip;
        
        audioSource.Play();
        
        float clipLength =  audioSource.clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlaySoundRandom(AudioClip[] _clips, Transform _spawnTransform, float _volume = 1f)
    {
        Random.Range(0,  _clips.Length);
        
        PlaySound(_clips[Random.Range(0, _clips.Length)], _spawnTransform, _volume);
    }
    
    // TBA !! Play sound while (f.ex. background music)
    // TBA !! Play reandom sounds with random pitch (f.ex footsteps)

}
