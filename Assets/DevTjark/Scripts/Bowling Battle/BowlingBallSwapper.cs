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

    public BallType GetCurrentBallType() => m_currentBallType;

    public void SwapToBasketball() => SwapBall(BallType.Basketball, basketBallCollider);
    public void SwapToBaseball()   => SwapBall(BallType.Baseball, baseBallCollider);
    public void SwapToFootball()   => SwapBall(BallType.Football, footballCollider);

    public void SwapToNextBall(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        ++m_currentBallIndex;
        if (m_currentBallIndex >= m_ballColliders.Length - 1) m_currentBallIndex = 0;
        SwapBall((BallType)m_currentBallIndex, m_ballColliders[m_currentBallIndex]);
    }

    public void SwapToPreviousBall(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        --m_currentBallIndex;
        if (m_currentBallIndex < 0) m_currentBallIndex = m_ballColliders.Length - 2;
        SwapBall((BallType)m_currentBallIndex, m_ballColliders[m_currentBallIndex]);
    }

    public void SwapToBall(int _index) => SwapBall((BallType)_index, m_ballColliders[_index]);

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