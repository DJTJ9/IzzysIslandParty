using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CharacterCreator
{
    public class CharacterCreatorService : MonoBehaviour
    {
        [SerializeField] private SO_MaterialCollection characterMaterialCollection;
        [SerializeField] private SO_MeshCollection characterMeshCollection;

        [SerializeField] private bool hasTwoMeshes;

        [ShowIf("hasTwoMeshes")]
        [SerializeField] private SO_MaterialCollection OtherMaterialCollection;

        public void SetMeshAndMaterial(SkinnedMeshRenderer _meshRenderer, int _playerIndex)
        {
            _meshRenderer.sharedMesh = characterMeshCollection.Meshes[_playerIndex];
            _meshRenderer.sharedMaterial = characterMaterialCollection.Materials[_playerIndex];
        }

        public void SetOtherMaterial(MeshRenderer _meshRenderer, int _playerIndex)
        {
            if (!hasTwoMeshes)
                return;

            _meshRenderer.sharedMaterial = OtherMaterialCollection.Materials[_playerIndex];
        }
    }
}