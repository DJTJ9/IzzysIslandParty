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
    [SerializeField] private SO_SwaggySnapshotsPlayerCollection currentPlayers;
    // [SerializeField] private GameScoreSO scoreSO;
    [SerializeField] private FaceSwapSO faceSwapSO;
    [SerializeField] private DanceMovesSO danceMovesSO;
    
    private Vector3 m_faceDirection;
    private float m_currentYRotation;
    private Vector3 m_cameraDirection;
    private bool m_turnedAwayFromCamera;

    private void OnEnable()
    {
        PlayerControllerSwaggySnapshots.onTakePhoto += CalculatePoints;
    }
    
    private void OnDisable()
    {
        PlayerControllerSwaggySnapshots.onTakePhoto -= CalculatePoints;
    }

    /// <summary>
    /// Calculates points for a player when a photo is taken, considering three factors:
    /// - Face direction.
    /// - Face expression.
    /// - Dance move being performed.
    /// </summary>
    /// <param name="_playerIndex">The index of the player for whom points are calculated.</param>
    public void CalculatePoints(int _playerIndex)
    {
        CalculatePointsForFaceDirection(_playerIndex);
        CalculatePointsForFaceExpression(_playerIndex);
        CalculatePointsForDanceMove(_playerIndex);
    }

    /// <summary>
    /// Awards points to the player if their face is directed towards the camera.
    /// </summary>
    /// <param name="_playerIndex">The index of the player to evaluate.</param>
    private void CalculatePointsForFaceDirection(int _playerIndex)
    {
        if (m_turnedAwayFromCamera) return;
        
        currentPlayers.Players[_playerIndex].PlayerScore.Value += m_faceForwardPointValue;
        currentPlayers.Players[_playerIndex].FacingCameraScore += m_faceForwardPointValue;
    }

    /// <summary>
    /// Awards points to the player if their face has a "happy" expression, based on material evaluation.
    /// </summary>
    /// <param name="_playerIndex">The index of the player to evaluate.</param>
    private void CalculatePointsForFaceExpression(int _playerIndex)
    {
        var currentFaceMaterial = faceMeshRenderer.sharedMaterial;
        var isHappyFace = faceSwapSO.IsHappyFace(currentFaceMaterial);

        if (!isHappyFace) return;
        
        currentPlayers.Players[_playerIndex].PlayerScore.Value += m_happyFacePointValue;
        currentPlayers.Players[_playerIndex].HappyFaceScore += m_happyFacePointValue;
    }

    /// <summary>
    /// Awards points to the player if they are performing a dance move classified as "cool."
    /// </summary>
    /// <param name="_playerIndex">The index of the player to evaluate.</param>
    private void CalculatePointsForDanceMove(int _playerIndex)
    {
        var currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        if (!danceMovesSO.IsCoolDanceMove(currentClip)) return;
        
        currentPlayers.Players[_playerIndex].PlayerScore.Value += m_danceMovePointValue;
        currentPlayers.Players[_playerIndex].CoolDanceMoveScore += m_danceMovePointValue;
    }
    
    /// <summary>
    /// Sets the state indicating that the player has turned away from the camera.
    /// </summary>
    public void SetTurnedAwayFromCameraToTrue() => m_turnedAwayFromCamera = true;
    
    /// <summary>
    /// Resets the state, indicating that the player is no longer turned away from the camera.
    /// </summary>
    public void SetTurnedAwayFromCameraToFalse() => m_turnedAwayFromCamera = false;
}
