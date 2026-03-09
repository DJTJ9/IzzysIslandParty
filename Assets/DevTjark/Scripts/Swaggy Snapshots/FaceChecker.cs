using UnityEngine;

public class FaceChecker : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public SkinnedMeshRenderer SkinnedMeshRenderer => skinnedMeshRenderer;
    
    private Material m_currentMaterial;
}
