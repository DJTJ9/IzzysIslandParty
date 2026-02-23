using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Afterimage")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/afterimage/")]
    public class AfterimageSettings : SnapshotVolumeComponent
    {
        public AfterimageSettings()
        {
            displayName = "Afterimage";
        }

        [Tooltip("How should the afterimage be drawn? Off = no afterimage, Masked = only masked objects, Everywhere = the entire screen uses an afterimage, even if a mask is set.")]
        public AfterimageModeParameter afterimageMode = new AfterimageModeParameter(AfterimageMode.Off);

        [Tooltip("What proportion of the next drawn frame should be made up of the previous frame? Larger values cause a 'laggier' screen.")]
        public ClampedFloatParameter persistence = new ClampedFloatParameter(0.0f, 0.0f, 0.999f);

        public override bool IsActive() => (afterimageMode.value != AfterimageMode.Off && active);
    }
}

