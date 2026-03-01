using System.Collections;
using Sirenix.OdinInspector;
using SnapshotShaders.URP;
using UnityEngine;
using UnityEngine.Rendering;

public class GameViewTransition : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float startValue;
    [SerializeField] private float endValue;
    [SerializeField] private float duration;
    
    [SerializeField] private Volume volume;
    private CutoutSettings cutoutSettings;

    private void Start()
    {
        // Suche nach einem Global Volume, das die CutoutSettings enthält
        // var volume = FindAnyObjectByType<Volume>();
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out cutoutSettings);
        }
        else
        {
            Debug.LogError("Volume oder VolumeProfile fehlt in der Szene.");
        }
    }

    public void StartGameViewTransition() => StartCoroutine(AnimateGameViewTransition());

    private IEnumerator AnimateGameViewTransition()
    {
        if (cutoutSettings == null)
        {
            yield break;
        }

        cutoutSettings.zoom.overrideState = true;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            var t = Mathf.Clamp01(elapsedTime / duration);                 
            cutoutSettings.zoom.value = Mathf.Lerp(startValue, endValue, t); 

            yield return null;
        }

        cutoutSettings.zoom.value = endValue;
    }

}
