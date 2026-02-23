using UnityEditor;
using UnityEditor.Rendering;

namespace DanielIlett.SnapshotShaders2.URP.Editor
{
    [CustomEditor(typeof(AfterimageSettings))]
    public class AfterimageEditor : SnapshotVolumeComponentEditor
    {
        SerializedDataParameter afterimageMode;
        SerializedDataParameter persistence;

        protected override string BannerTextureName { get { return "AfterimageBanner"; } }

        public override void OnEnable()
        {
            var o = new PropertyFetcher<AfterimageSettings>(serializedObject);
            FetchBasicParameters(o);

            afterimageMode = Unpack(o.Find(x => x.afterimageMode));
            persistence = Unpack(o.Find(x => x.persistence));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawBanner();

            DrawBasicSettings<AfterimageFeature>(true);

            DrawDivider();

            EditorGUILayout.LabelField("Afterimage Settings", HeaderStyle);
            PropertyField(afterimageMode);

            if(afterimageMode.value.GetEnumValue<AfterimageMode>() != AfterimageMode.Off)
            {
                PropertyField(persistence);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
