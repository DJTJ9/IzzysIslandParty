using UnityEditor;
using UnityEditor.Rendering;

namespace DanielIlett.SnapshotShaders2.URP.Editor
{
    [CustomEditor(typeof(OutlineSettings))]
    public class OutlineEditor : SnapshotVolumeComponentEditor
    {
        SerializedDataParameter enabled;
        SerializedDataParameter outlineAlgorithm;
        SerializedDataParameter outlineColor;
        SerializedDataParameter colorThreshold;
        SerializedDataParameter colorStrength;
        SerializedDataParameter depthThreshold;
        SerializedDataParameter depthStrength;
        SerializedDataParameter normalThreshold;
        SerializedDataParameter normalStrength;
        SerializedDataParameter skyboxDepthCutoff;
        SerializedDataParameter drawingMode;
        SerializedDataParameter backgroundColor;
        SerializedDataParameter neonSaturationFloor;
        SerializedDataParameter neonLightnessFloor;

        protected override string BannerTextureName { get { return "OutlineBanner"; } }

        public override void OnEnable()
        {
            var o = new PropertyFetcher<OutlineSettings>(serializedObject);
            FetchBasicParameters(o);

            enabled = Unpack(o.Find(x => x.enabled));
            outlineAlgorithm = Unpack(o.Find(x => x.outlineAlgorithm));
            outlineColor = Unpack(o.Find(x => x.outlineColor));
            colorThreshold = Unpack(o.Find(x => x.colorThreshold));
            colorStrength = Unpack(o.Find(x => x.colorStrength));
            depthThreshold = Unpack(o.Find(x => x.depthThreshold));
            depthStrength = Unpack(o.Find(x => x.depthStrength));
            normalThreshold = Unpack(o.Find(x => x.normalThreshold));
            normalStrength = Unpack(o.Find(x => x.normalStrength));
            skyboxDepthCutoff = Unpack(o.Find(x => x.skyboxDepthCutoff));
            drawingMode = Unpack(o.Find(x => x.drawingMode));
            backgroundColor = Unpack(o.Find(x => x.backgroundColor));
            neonSaturationFloor = Unpack(o.Find(x => x.neonSaturationFloor));
            neonLightnessFloor = Unpack(o.Find(x => x.neonLightnessFloor));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawBanner();

            DrawBasicSettings<OutlineFeature>(true);

            DrawDivider();

            EditorGUILayout.LabelField("Outline Settings", HeaderStyle);
            PropertyField(enabled);
            PropertyField(outlineAlgorithm);
            PropertyField(outlineColor);
            PropertyField(colorThreshold);
            PropertyField(colorStrength);
            PropertyField(depthThreshold);
            PropertyField(depthStrength);
            PropertyField(normalThreshold);
            PropertyField(normalStrength);
            PropertyField(skyboxDepthCutoff);
            PropertyField(drawingMode);

            var olMode = drawingMode.value.GetEnumValue<OutlineDrawingMode>();

            if(olMode == OutlineDrawingMode.NeonOnly || olMode == OutlineDrawingMode.OutlinesOnly)
            {
                PropertyField(backgroundColor);
            }

            if (olMode == OutlineDrawingMode.NeonOnly || olMode == OutlineDrawingMode.NeonOverlay)
            {
                PropertyField(neonSaturationFloor);
                PropertyField(neonLightnessFloor);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
