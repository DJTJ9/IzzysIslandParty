using UnityEngine;
using UnityEngine.Rendering;

namespace DanielIlett.SnapshotShaders2.URP
{
    [System.Serializable, VolumeComponentMenu("Snapshot Shaders 2/Filmic")]
    [HelpURL("https://danielilett.com/snapshot-shaders-2/filmic/")]
    public sealed class FilmicSettings : SnapshotVolumeComponent
    {
        public FilmicSettings()
        {
            displayName = "Filmic";
        }

        [Tooltip("Is the effect active?")]
        public BoolParameter useFilmBars = new(false);

        [Tooltip("Desired aspect ratio , expressed as (x:y), e.g. (16:9).")]
        public Vector2Parameter aspectRatio = new(new Vector2(16, 9));

        [Tooltip("Color of the film bars.")]
        public ColorParameter filmBarColor = new(Color.black);

        [Tooltip("How should the noise be displayed on-screen? Modulate will offset existing screen colors, Overlay will replace pixels that exceed the threshold with pure white, Off won't display any noise.")]
        public NoiseDisplayModeParameter noiseMode = new(NoiseDisplayMode.Off);

        [Tooltip("How strongly the screen colors get lightened by noise.")]
        public ClampedFloatParameter noiseStrength = new(0.0f, 0.0f, 1.0f);

        [Tooltip("How fast the noise grain changes values.")]
        public FloatParameter noiseSpeed = new(1.0f);

        [Tooltip("The size of the noise texture that gets applied to the screen.")]
        public ClampedFloatParameter noiseSize = new(1.0f, 0.1f, 5.0f);

        [Tooltip("Only noise values above this brightness level will appear.")]
        public ClampedFloatParameter noiseThreshold = new(0.0f, 0.0f, 1.0f);

        [Tooltip("Hermite interpolation is faster, while Quintic interpolation will look very slightly nicer.")]
        public NoiseInterpModeParameter noiseInterpolation = new(NoiseInterpolationMode.Quintic);

        public override bool IsActive() => (useFilmBars.value || noiseStrength.value > Mathf.Epsilon) && active;

        private void OnValidate()
        {
            var aspectRatioVector = aspectRatio.value;
            aspectRatioVector.x = Mathf.Max(aspectRatioVector.x, 0.01f);
            aspectRatioVector.y = Mathf.Max(aspectRatioVector.y, 0.01f);
            aspectRatio.value = aspectRatioVector;
        }
    }
}
