using UnityEngine;

public class AudioCollection : MonoBehaviour
{
    private static AudioCollection instance;
    public static AudioCollection Instance => instance;

    public SO_SoundDictionary levelSoundsDictionary;

    private AudioCollection()
    {
        instance = this;
    }

    private void Awake()
    {
        if (levelSoundsDictionary == null || levelSoundsDictionary.LevelAudios.Count < 1)
            Debug.LogError("No level sounds found");
    }
}