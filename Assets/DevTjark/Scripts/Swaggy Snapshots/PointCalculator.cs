using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class PointCalculator : MonoBehaviour
{
    [FoldoutGroup("Face Point Settings", expanded: true)]
    [SerializeField] private float m_faceForwardPointValue = 1f;
    // [SerializeField] private float m_lookAwayAngleTreshold = 45f;
    // [SerializeField] private float minRotationAngle = -45f;
    // [SerializeField] private float maxRotationAngle = -135f;
    // [SerializeField] private Camera mainCamera;
    
    [FoldoutGroup("Face Expression Settings", expanded: true)]
    [SerializeField] private float m_happyFacePointValue = 1f;
    [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
    
    [FoldoutGroup("Dance Move Settings", expanded: true)]
    [SerializeField] private float m_danceMovePointValue = 1f;
    [SerializeField] private Animator animator;
    
    [FoldoutGroup("Scriptable Objects", expanded: true)]
    [SerializeField] private SO_PlayerCollectionSwaggySnapshots playerCollectionSO;
    // [SerializeField] private GameScoreSO scoreSO;
    [SerializeField] private FaceSwapSO faceSwapSO;
    [SerializeField] private DanceMovesSO danceMovesSO;
    
    private Vector3 m_faceDirection;
    private float m_currentYRotation;
    private Vector3 m_cameraDirection;
    private bool m_turnedAwayFromCamera;

    // private void Awake()
    // {
    //     m_faceDirection = transform.forward;
    //     m_cameraDirection = (mainCamera.transform.position - transform.position).normalized;
    // }
    
    // private void Update()
    // {
    //     m_faceDirection = transform.forward;
    //     m_cameraDirection = (mainCamera.transform.position - transform.position).normalized;
    //     // m_currentYRotation = transform.eulerAngles.y;
    // }

    public void CalculatePoints(int _playerIndex)
    {
        CalculatePointsForFaceDirection(_playerIndex);
        CalculatePointsForFaceExpression(_playerIndex);
        CalculatePointsForDanceMove(_playerIndex);
    }

    private void CalculatePointsForFaceDirection(int _playerIndex)
    {
        // var dotProduct = Vector3.Dot(m_faceDirection, m_cameraDirection);
        // m_turnedAwayFromCamera = dotProduct < Mathf.Cos(Mathf.Deg2Rad * m_lookAwayAngleTreshold);
        
        if (m_turnedAwayFromCamera) return;
        
        playerCollectionSO.Players[_playerIndex].PlayerScore.Value += m_faceForwardPointValue;
        Debug.Log("Points added for facing the camera!");
    }

    private void CalculatePointsForFaceExpression(int _playerIndex)
    {
        var currentFaceMaterial = faceMeshRenderer.sharedMaterial;
        var isHappyFace = faceSwapSO.IsHappyFace(currentFaceMaterial);

        if (!isHappyFace) return;
        
        playerCollectionSO.Players[_playerIndex].PlayerScore.Value += m_happyFacePointValue;
        Debug.Log("Points added for happy face!");
    }

    private void CalculatePointsForDanceMove(int _playerIndex)
    {
        var currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        
        if (!danceMovesSO.IsCoolDanceMove(currentClip)) return;
        
        playerCollectionSO.Players[_playerIndex].PlayerScore.Value += m_danceMovePointValue;
        Debug.Log("Points added for cool dance move!");
    }
    
    public void SetTurnedAwayFromCameraToTrue() => m_turnedAwayFromCamera = true;
    public void SetTurnedAwayFromCameraToFalse() => m_turnedAwayFromCamera = false;

    // private bool isFacingCamera()
    // {
    //     return m_currentYRotation >= maxRotationAngle && m_currentYRotation <= minRotationAngle;
    // }
}
