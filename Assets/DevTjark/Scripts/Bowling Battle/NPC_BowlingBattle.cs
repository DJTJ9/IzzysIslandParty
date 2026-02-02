using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class NPC_BowlingBattle : Controller
{
    public int NPCIndex;

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float directionChangeInterval = 2f;

    [Header("References")]
    [SerializeField] private SO_Player npcSO;
    [SerializeField] private SO_PlayerCollection npcCollectionSO;
    
    private CharacterController controller;
    private Rigidbody rb;
    private BowlingBallSwapper bowlingBallSwapper;
    
    private float m_changeDirectionTimer = 0f;
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

    private void GameStartConfiguration()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.position = npcCollectionSO.Players[NPCIndex].SpawnPoint;
        transform.rotation = Quaternion.identity;

        rb.freezeRotation = true;
        rb.useGravity = false;
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
        m_changeDirectionTimer += Time.deltaTime;
        
        if (m_changeDirectionTimer >= directionChangeInterval)
        {
            var randomX = Random.Range(-1f, 1f);
            var randomY = Random.Range(-1f, 1f);
            m_moveInput = new Vector2(randomX, randomY).normalized;
            m_changeDirectionTimer = 0f;
        }
    }

    public void ChooseRandomBall()
    {
        var ballTypes = (BallType[])System.Enum.GetValues(typeof(BallType));
        var randomIndex = Random.Range(0, ballTypes.Length);
        var randomBallType = ballTypes[randomIndex];
        
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
        
        transform.position = npcCollectionSO.Players[NPCIndex].SpawnPoint;
        transform.rotation = Quaternion.identity;

        controller.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
    }
    
    public void EnableCharacterController() => controller.enabled = true;
    
    public SO_Player GetNpcSo => npcSO;
}