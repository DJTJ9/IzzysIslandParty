using System;
using UnityEngine;

public class DanceMoveManager : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer skinnedMeshRenderer;
    
    [SerializeField]
    private DanceMoveTriggersSO danceMoveTriggersSO;
    [SerializeField]
    private FaceSwapSO faceSwapSO;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void ChangeDanceMove()
    {
        m_animator.SetTrigger(danceMoveTriggersSO.GetRandomDanceMove());
    }

    public void ChangeFace()
    {
        skinnedMeshRenderer.material = faceSwapSO.GetRandomFace(skinnedMeshRenderer.material);
    }
}