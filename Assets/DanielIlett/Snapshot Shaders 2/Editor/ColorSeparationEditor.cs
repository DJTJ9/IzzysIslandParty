using UnityEditor;
using UnityEditor.Rendering;

namespace DanielIlett.SnapshotShaders2.URP.Editor
{
    [CustomEditor(typeof(ColorSeparationSettings))]
    public class ColorSeparationEditor : SnapshotVolumeComponentEditor
    {
        SerializedDataParameter separationOffset;

        protected override string BannerTextureName { get { return "ColorSeparationBanner"; } }

        public override void OnEnable()
        {
            var o = new PropertyFetcher<ColorSeparationSettings>(serializedObject);
            FetchBasicParameters(o);

            separationOffset = Unpack(o.Find(x => x.separationOffset));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawBanner();

            DrawBasicSettings<ColorSeparationFeature>(true);

            DrawDivider();

            EditorGUILayout.LabelField("Color Separation Settings", HeaderStyle);
            PropertyField(separationOffset);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
