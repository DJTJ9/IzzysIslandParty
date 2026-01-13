using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PointCalculator : MonoBehaviour
{
    [FoldoutGroup("Face Point Settings", expanded: true)]
    [SerializeField] private float m_faceForwardPointValue = 1f;
    [SerializeField] private float m_lookAwayAngleTreshold = 45f;
    [SerializeField] private float minRotationAngle = -45f;
    [SerializeField] private float maxRotationAngle = -135f;
    [SerializeField] private Camera mainCamera;
    
    [FoldoutGroup("Face Expression Settings", expanded: true)]
    [SerializeField] private float m_happyFacePointValue = 1f;
    [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
    
    [FoldoutGroup("Dance Move Settings", expanded: true)]
    [SerializeField] private float m_danceMovePointValue = 1f;
    [SerializeField] private Animator animator;
    
    [FoldoutGroup("Scriptable Objects", expanded: true)]
    [SerializeField] private GameScoreSO scoreSO;
    [SerializeField] private FaceSwapSO faceSwapSO;
    [SerializeField] private DanceMovesSO danceMovesSO;
    
    private Vector3 m_faceDirection;
    private float m_currentYRotation;
    private Vector3 m_cameraDirection;
    private bool m_turnedAwayFromCamera;

    private void Awake()
    {
        m_faceDirection = transform.forward;
        m_cameraDirection = (mainCamera.transform.position - transform.position).normalized;
    }
    
    private void Update()
    {
        m_faceDirection = transform.forward;
        m_cameraDirection = (mainCamera.transform.position - transform.position).normalized;
        // m_currentYRotation = transform.eulerAngles.y;
    }

    public void CalculatePoints()
    {
        CalculatePointsForFaceDirection();
        CalculatePointsForFaceExpression();
        CalculatePointsForDanceMove();
    }

    private void CalculatePointsForFaceDirection()
    {
        var dotProduct = Vector3.Dot(m_faceDirection, m_cameraDirection);
        m_turnedAwayFromCamera = dotProduct < Mathf.Cos(Mathf.Deg2Rad * m_lookAwayAngleTreshold);
        
        if (m_turnedAwayFromCamera) return;
        
        scoreSO.Value += m_faceForwardPointValue;
        Debug.Log("Points added for facing the camera!");
    }

    private void CalculatePointsForFaceExpression()
    {
        var currentFaceMaterial = faceMeshRenderer.materials[0];
        var isHappyFace = faceSwapSO.IsHappyFace(currentFaceMaterial);

        if (!isHappyFace) return;
        
        scoreSO.Value += m_happyFacePointValue;
        Debug.Log("Points added for happy face!");
    }

    private void CalculatePointsForDanceMove()
    {
        var currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        
        if (!danceMovesSO.IsCoolDanceMove(currentClip)) return;
        
        scoreSO.Value += m_danceMovePointValue;
        Debug.Log("Points added for cool dance move!");
    }

    private bool isFacingCamera()
    {
        return m_currentYRotation >= maxRotationAngle && m_currentYRotation <= minRotationAngle;
    }
}
