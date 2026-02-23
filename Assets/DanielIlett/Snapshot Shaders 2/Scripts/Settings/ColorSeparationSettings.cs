using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Color Separation")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/color-separation/")]
    public class ColorSeparationSettings : SnapshotVolumeComponent
    {
        public ColorSeparationSettings()
        {
            displayName = "Color Separation";
        }

        [Tooltip("Direction in which the blue channel moves in UV space. The red channel moves in the opposite direction.")]
        public Vector2Parameter separationOffset = new(Vector2.zero);

        public override bool IsActive() => separationOffset.value.magnitude > Mathf.Epsilon && active;
    }
}

