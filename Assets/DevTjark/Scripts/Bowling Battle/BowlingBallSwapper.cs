using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowlingBallSwapper : MonoBehaviour
{
    [SerializeField] private SO_BowlingBallMeshes bowlingBallMeshes;
    [SerializeField] private SO_BowlingBallMaterials bowlingBallMaterials;
    
    [FoldoutGroup("Colliders", expanded: false)]
    [SerializeField] private SphereCollider basketBallCollider;
    [SerializeField] private SphereCollider baseBallCollider;
    [SerializeField] private MeshCollider footballCollider;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Collider[] m_ballColliders = new Collider[Enum.GetValues(typeof(BallType)).Length];
    private BallType m_currentBallType = BallType.Baseball;
    private Collider m_currentCollider;
    private int m_currentBallIndex;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        m_ballColliders = GetComponents<Collider>();
        m_currentCollider = basketBallCollider;
    }

    /// <summary>
    /// Retrieves the currently active ball type.
    /// </summary>
    /// <returns>The current BallType.</returns>
    public BallType GetCurrentBallType() => m_currentBallType;

    /// <summary>
    /// Switches the active ball type and collider to Basketball.
    /// Updates the mesh and material accordingly.
    /// </summary>
    public void SwapToBasketball() => SwapBall(BallType.Basketball, basketBallCollider);
    
    /// <summary>
    /// Switches the active ball type and collider to Baseball.
    /// Updates the mesh and material accordingly.
    /// </summary>
    public void SwapToBaseball()   => SwapBall(BallType.Baseball, baseBallCollider);
    
    /// <summary>
    /// Switches the active ball type and collider to Football.
    /// Updates the mesh and material accordingly.
    /// </summary>
    public void SwapToFootball()   => SwapBall(BallType.Football, footballCollider);
    
    /// <summary>
    /// Swaps to the next ball type in the list when an input action is triggered.
    /// If at the end of the list, loops back to the beginning.
    /// </summary>
    /// <param name="_context">The input action context.</param>
    public void SwapToNextBall(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        ++m_currentBallIndex;
        if (m_currentBallIndex >= m_ballColliders.Length - 1) m_currentBallIndex = 0;
        SwapBall((BallType)m_currentBallIndex, m_ballColliders[m_currentBallIndex]);
    }

    /// <summary>
    /// Swaps to the previous ball type in the list when an input action is triggered.
    /// If at the beginning of the list, loops back to the end.
    /// </summary>
    /// <param name="_context">The input action context.</param>
    public void SwapToPreviousBall(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        --m_currentBallIndex;
        if (m_currentBallIndex < 0) m_currentBallIndex = m_ballColliders.Length - 2;
        SwapBall((BallType)m_currentBallIndex, m_ballColliders[m_currentBallIndex]);
    }

    /// <summary>
    /// Swaps to a specific ball type by index.
    /// Updates the active collider, mesh, and material accordingly.
    /// </summary>
    /// <param name="_index">The index of the ball type to switch to.</param>
    public void SwapToBall(int _index) => SwapBall((BallType)_index, m_ballColliders[_index]);

    /// <summary>
    /// Handles the logic for swapping to a specified ball type and collider.
    /// Updates the current mesh, material, and enables the correct collider.
    /// </summary>
    /// <param name="_type">The desired BallType to switch to.</param>
    /// <param name="_collider">The collider associated with the BallType.</param>
    private void SwapBall(BallType _type, Collider _collider)
    {
        meshFilter.mesh = bowlingBallMeshes.BallMeshes[_type];
        meshRenderer.material = bowlingBallMaterials.BallMaterials[_type];
        m_currentCollider.enabled = false;

        m_currentCollider = _collider;
        m_currentCollider.enabled = true;
        m_currentBallType = _type;
    }
}