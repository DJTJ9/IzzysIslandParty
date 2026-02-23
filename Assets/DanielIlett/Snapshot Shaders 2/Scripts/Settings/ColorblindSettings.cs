using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Colorblind")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/colorblind/")]
    public class ColorblindSettings : SnapshotVolumeComponent
    {
        public ColorblindSettings()
        {
            displayName = "Colorblind";
        }

        [Tooltip("Which type of color-blindness to simulate.")]
        public ColorblindModeParameter colorblindMode = new(ColorblindMode.None);

        [Tooltip("How strongly the filter is applied to the screen.")]
        public ClampedFloatParameter strength = new(0.0f, 0.0f, 1.0f);

        public override bool IsActive() => (colorblindMode.value != ColorblindMode.None && 
            strength.value > Mathf.Epsilon && active);
    }
}

