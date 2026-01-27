using UnityEngine;
using UnityEngine.InputSystem;

public class NPC_BowlingBattle : MonoBehaviour
{
    public int PlayerIndex;
    
    [Header("Input")]

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float directionChangeInterval = 2f;
    
    private InputAction m_moveInputAction;
    private InputAction m_jumpInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;

    [Header("References")]
    [SerializeField] private SO_PlayerCollection playerCollectionSo;
    
    private CharacterController controller;
    private Rigidbody rb;
    private BowlingBallSwapper bowlingBallSwapper;
    
    private float changeDirectionTimer = 0f;
    private Vector2 m_moveInput;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        bowlingBallSwapper = GetComponent<BowlingBallSwapper>();
    }

    private void OnEnable()
    {
        GameStartConfiguration();
    }
    
    private void FixedUpdate()
    {
        if (controller.enabled) Movement();
    }

    public void GameStartConfiguration()
    {
        ResetComponents();
    }
    
    private void Movement()
    {
        StartPositionMovement();
    }

    private void StartPositionMovement()
    {
        GetMoveDirection();
        var move = new Vector3(m_moveInput.x, m_moveInput.y, 0);

        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    private void GetMoveDirection()
    {
        changeDirectionTimer += Time.deltaTime;
        
        if (changeDirectionTimer >= directionChangeInterval)
        {
            var randomX = Random.Range(-1f, 1f);
            var randomY = Random.Range(-1f, 1f);
            m_moveInput = new Vector2(randomX, randomY).normalized;
            changeDirectionTimer = 0f;
        }
    }

    public void ChooseRandomBall()
    {
        var ballTypes = (BallType[])System.Enum.GetValues(typeof(BallType));
        var randomIndex = Random.Range(0, ballTypes.Length);
        var randomBallType = ballTypes[randomIndex];
        
        Debug.Log($"Random ball type: {randomBallType}");

        switch (randomBallType)
        {
            case BallType.Baseball:
                bowlingBallSwapper.SwapToBaseball();
                break;
            case BallType.Basketball:
                bowlingBallSwapper.SwapToBasketball();
                break;
            case BallType.Football:
                bowlingBallSwapper.SwapToFootball();
                break;
            default: 
                bowlingBallSwapper.SwapToBasketball(); 
                break;
        }
    }
    
    public void OnReleaseBall()
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            m_moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    public void ResetComponents()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.position = playerCollectionSo.Players[PlayerIndex].SpawnPoint;
        transform.rotation = Quaternion.identity;


        controller.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
    }
}