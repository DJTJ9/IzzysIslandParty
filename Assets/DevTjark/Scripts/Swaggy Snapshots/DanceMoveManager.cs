using UnityEngine;

public class DanceMoveManager : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField] private DanceMoveTriggersSO danceMoveTriggersSO;
    [SerializeField] private FaceSwapSO faceSwapSO;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.Play("Start_Move");
    }

    /// <summary>
    /// Updates the animator to trigger a random dance move based on predefined triggers from the scriptable object.
    /// </summary>
    public void ChangeDanceMove()
    {
        m_animator.SetTrigger(danceMoveTriggersSO.GetRandomDanceMove());
    }

    /// <summary>
    /// Changes the skinned mesh renderer's material to a random face material,
    /// chosen from a set defined in the scriptable object.
    /// </summary>
    public void ChangeFace()
    {
        skinnedMeshRenderer.material = faceSwapSO.GetRandomFace(skinnedMeshRenderer.material);
    }
}