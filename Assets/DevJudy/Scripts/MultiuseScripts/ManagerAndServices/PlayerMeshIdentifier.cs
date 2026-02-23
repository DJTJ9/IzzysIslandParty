using Sirenix.OdinInspector;
using UnityEngine;

namespace CharacterCreator
{
    public class PlayerMeshIdentifier : MonoBehaviour
    {
        [field: SerializeField] public SkinnedMeshRenderer MeshRenderer { get; private set; }
        
        [SerializeField] public bool IsJetski;

        [ShowIf("IsJetski")]
        [SerializeField] public MeshRenderer JetskiMeshRenderer;
    }
}