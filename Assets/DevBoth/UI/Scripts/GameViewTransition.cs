using SnapshotShaders.URP;
using UnityEngine;
using UnityEngine.Rendering;

public class GameViewTransition : MonoBehaviour
{
    public float newZoomValue = 1.5f;

    private void Update()
    {
        if (VolumeManager.instance == null)
        {
            Debug.LogError("VolumeManager.instance ist null. Ist ein Volume in der Szene?");
            return;
        }
        
        if (VolumeManager.instance.stack == null)
        {
            Debug.LogError("VolumeManager.instance.stack ist null. Es gibt keinen aktiven Volume Stack. Ist das Volume korrekt konfiguriert?");
            return;
        }

        // Hole die CutoutSettings vom VolumeManager
        var cutoutSettings = VolumeManager.instance.stack.GetComponent<CutoutSettings>();
        
        if (cutoutSettings == null)
        {
            Debug.LogError("CutoutSettings sind nicht im Volume Profile konfiguriert.");
            return;
        }

        if (cutoutSettings == null)
        {
            Debug.LogError("CutoutSettings sind nicht im Volume Profile konfiguriert.");
            return;
        }

        // Aktiv prüfen und Zoom-Wert setzen
        if (cutoutSettings.IsActive())
        {
            cutoutSettings.zoom.overrideState = true; // Override zulassen
            cutoutSettings.zoom.value = newZoomValue; // Zoom-Wert setzen
            Debug.Log($"Zoom-Wert auf {newZoomValue} gesetzt.");
        }

        else
        {
            Debug.LogWarning("CutoutSettings konnten nicht gefunden werden oder sind nicht aktiv.");
        }
    }

}
