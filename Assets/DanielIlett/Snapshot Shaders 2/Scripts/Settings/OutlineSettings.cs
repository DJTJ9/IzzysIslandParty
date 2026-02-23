using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Outline")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/outline/")]
    public class OutlineSettings : SnapshotVolumeComponent
    {
        public OutlineSettings()
        {
            displayName = "Outline";
        }

        [Tooltip("Is the effect enabled?")]
        public BoolParameter enabled = new BoolParameter(false);

        [Tooltip("Which outline should the shader to use to detect outlines?")]
        public OutlineAlgorithmParameter outlineAlgorithm = new OutlineAlgorithmParameter(OutlineAlgorithm.DepthNormalsColor);

        [Tooltip("Color to use for the outlines. The alpha component acts as a global multiplier for outline strength.")]
        public NoInterpColorParameter outlineColor = new NoInterpColorParameter(Color.white, true, true, true);

        [Tooltip("Adjacent colors with differences over this threshold will be edge-detected.")]
        public ClampedFloatParameter colorThreshold = new ClampedFloatParameter(0.1f, 0.0f, 1.0f);

        [Tooltip("How strongly color-based edge detection factors into the overall edge strength calculation.")]
        public ClampedFloatParameter colorStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("Adjacent depth values with differences over this threshold will be edge-detected.")]
        public ClampedFloatParameter depthThreshold = new ClampedFloatParameter(0.1f, 0.0f, 1.0f);

        [Tooltip("How strongly depth-based edge detection factors into the overall edge strength calculation.")]
        public ClampedFloatParameter depthStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("Adjacent normal vectors with direction differences over this threshold will be edge-detected.")]
        public ClampedFloatParameter normalThreshold = new ClampedFloatParameter(0.1f, 0.0f, 1.0f);

        [Tooltip("How strongly normal-based edge detection factors into the overall edge strength calculation.")]
        public ClampedFloatParameter normalStrength = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("Pixels with depth exceeding this threshold will not be edge-detected.")]
        public ClampedFloatParameter skyboxDepthCutoff = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("How should the shader draw the edges?" + 
            "\n\nOutline modes use the Outline Color, while Neon modes use the original pixel colors and may boost their brightness and saturation." + 
            "\n\nOverlay modes layer the outline onto the original image, while Only modes display only the outlines alone.")]
        public OutlineDrawingModeParameter drawingMode = new OutlineDrawingModeParameter(OutlineDrawingMode.OutlinesOverlay);

        [Tooltip("Color to use for the background in OutlinesOnly or NeonOnly drawing modes.")]
        public ColorParameter backgroundColor = new ColorParameter(Color.black);

        [Tooltip("When using neon colors, boost all pixels to use at least this saturation value.")]
        public ClampedFloatParameter neonSaturationFloor = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        [Tooltip("When using neon colors, boost all pixels to use at least this lightness value.")]
        public ClampedFloatParameter neonLightnessFloor = new ClampedFloatParameter(0.0f, 0.0f, 1.0f);

        public override bool IsActive() => (enabled.value && active);
    }
}

