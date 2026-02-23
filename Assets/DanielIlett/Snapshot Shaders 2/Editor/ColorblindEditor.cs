using UnityEditor;
using UnityEditor.Rendering;

namespace DanielIlett.SnapshotShaders2.URP.Editor
{
    [CustomEditor(typeof(ColorblindSettings))]
    public class ColorblindEditor : SnapshotVolumeComponentEditor
    {
        SerializedDataParameter colorblindMode;
        SerializedDataParameter strength;

        protected override string BannerTextureName { get { return "ColorblindBanner"; } }

        public override void OnEnable()
        {
            var o = new PropertyFetcher<ColorblindSettings>(serializedObject);
            FetchBasicParameters(o);

            colorblindMode = Unpack(o.Find(x => x.colorblindMode));
            strength = Unpack(o.Find(x => x.strength));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawBanner();

            DrawBasicSettings<ColorblindFeature>(true);

            DrawDivider();

            EditorGUILayout.LabelField("Colorblind Settings", HeaderStyle);
            PropertyField(colorblindMode);
            PropertyField(strength);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
