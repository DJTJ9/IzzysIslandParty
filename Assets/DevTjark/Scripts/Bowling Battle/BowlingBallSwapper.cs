using UnityEngine;

public class BowlingBallSwapper : MonoBehaviour
{
    [SerializeField] private SO_BowlingBallMeshes bowlingBallMeshes;
    [SerializeField] private SO_BowlingBallMaterials bowlingBallMaterials;

    [SerializeField] private SphereCollider baseBallCollider;
    [SerializeField] private SphereCollider basketBallCollider;
    [SerializeField] private MeshCollider footballCollider;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    private BallType m_currentBallType = BallType.Baseball;
    private Collider m_currentCollider;
    
    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        m_currentCollider = baseBallCollider;
    }
    
    public BallType GetCurrentBallType() => m_currentBallType;

    public void SwapToBaseball()   => SwapBall(BallType.Baseball, baseBallCollider);
    public void SwapToBasketball() => SwapBall(BallType.Basketball, basketBallCollider);
    public void SwapToFootball()   => SwapBall(BallType.Football, footballCollider);
    
    
    private void SwapBall(BallType _type, Collider _collider)
    {
        meshFilter.mesh = bowlingBallMeshes.BallMeshes[_type];
        meshRenderer.material = bowlingBallMaterials.BallMaterials[_type];
        
        m_currentCollider.enabled = false;
        m_currentCollider = _collider;
        _collider.enabled = true;
        
        m_currentBallType = _type;
    }
}