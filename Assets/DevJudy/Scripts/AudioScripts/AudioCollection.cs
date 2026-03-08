using UnityEngine;

public class AudioCollection : MonoBehaviour
{
    private static AudioCollection instance;
    public static AudioCollection Instance => instance;

    public SO_SoundDictionary LevelSoundsDictionary;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (LevelSoundsDictionary == null || LevelSoundsDictionary.LevelAudios.Count < 1)
            Debug.LogError("No level sounds found");
    }
}