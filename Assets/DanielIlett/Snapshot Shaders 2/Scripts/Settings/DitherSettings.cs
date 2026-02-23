using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Dither")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/dither/")]
    public class DitherSettings : SnapshotVolumeComponent
    {
        public DitherSettings()
        {
            displayName = "Dither";
        }

        [Tooltip("Choose whether the threshold is applied to the luminance of the screen color, or to " + 
            "the individual RGB channels of the screen color.")]
        public DitherModeParameter ditherMode = new DitherModeParameter(DitherMode.Luminance);

        [Tooltip("Texture pattern to use as dither thresholds.")]
        public TextureParameter noiseTexture = new TextureParameter(null);

        [Tooltip("Size of the dither pattern tiled across the screen.")]
        public NoInterpClampedFloatParameter noiseSize = new NoInterpClampedFloatParameter(1.0f, 0.01f, 100.0f);

        [Tooltip("An additional offset applied to each pixel before thresholding.")]
        [InspectorName("Threshold")] public NoInterpClampedFloatParameter luminanceThreshold = new NoInterpClampedFloatParameter(0.0f, -1.0f, 1.0f);

        [Tooltip("Color of the areas whose luminance or RGB values fall under the threshold.")]
        public ColorParameter darkColor = new ColorParameter(Color.black);

        [Tooltip("When enabled, the Light Color is replaced by the original screen color.")]
        public BoolParameter useSceneColor = new BoolParameter(false);

        [Tooltip("Color of the areas whose luminance or RGB values exceed the threshold.")]
        public ColorParameter lightColor = new ColorParameter(Color.white);

        public override bool IsActive() => (noiseTexture.value != null && active);
    }
}

