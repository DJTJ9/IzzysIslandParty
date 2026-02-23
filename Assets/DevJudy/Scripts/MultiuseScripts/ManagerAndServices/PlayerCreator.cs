using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CharacterCreator
{
    public class CharacterCreatorService : MonoBehaviour
    {
        [SerializeField] private SO_MaterialCollection characterMaterialCollection;
        [SerializeField] private SO_MeshCollection characterMeshCollection;

        [SerializeField] private bool isJetskiGame;

        [ShowIf("isJetskiGame")]
        [SerializeField] private SO_MaterialCollection jetskiMaterialCollection;

        public void SetMeshAndMaterial(SkinnedMeshRenderer _meshRenderer, int _playerIndex)
        {
            Debug.Log("SetMeshAndMaterial");
            _meshRenderer.sharedMesh = characterMeshCollection.Meshes[_playerIndex];
            _meshRenderer.sharedMaterial = characterMaterialCollection.Materials[_playerIndex];
        }

        public void SetJetskiMaterial(MeshRenderer _meshRenderer, int _playerIndex)
        {
            if (!isJetskiGame)
                return;

            _meshRenderer.sharedMaterial = jetskiMaterialCollection.Materials[_playerIndex];
        }
    }
}