using UnityEngine;

public class FaceSwapper : StateMachineBehaviour
{
    [SerializeField] private FaceSwapSO faceSwapSO;
    
    private SkinnedMeshRenderer m_skinnedMeshRenderer;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var meshRendererReference = animator.GetComponentInChildren<FaceChecker>();
        m_skinnedMeshRenderer = meshRendererReference.SkinnedMeshRenderer;
        m_skinnedMeshRenderer.material = faceSwapSO.GetRandomFace(m_skinnedMeshRenderer.material);
    }
}
