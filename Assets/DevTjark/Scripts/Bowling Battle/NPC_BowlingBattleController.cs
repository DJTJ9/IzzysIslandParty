using UnityEngine;

public class NPC_BowlingBattleController : Controller
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

    /// <summary>
    /// Determines and applies the NPC's movement based on a directional input,
    /// adjusting position gradually.
    /// </summary>
    private void StartPositionMovement()
    {
        GetMoveDirection();
        var move = new Vector3(m_moveInput.x, m_moveInput.y, 0);

        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    /// <summary>
    /// Generates and sets a new random direction for the NPC's movement
    /// at specified intervals.
    /// </summary>
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

    /// <summary>
    /// Swaps the NPC's ball to a randomly selected BallType,
    /// updating its mesh, material, and active collider.
    /// </summary>
    public void ChooseRandomBall()
    {
        var ballTypes = (BallType[])System.Enum.GetValues(typeof(BallType));
        var randomIndex = Random.Range(0, ballTypes.Length);
        
        bowlingBallSwapper.SwapToBall(randomIndex);
    }
    
    /// <summary>
    /// Disables player movement and simulates the release of the ball
    /// by enabling gravity and resetting Rigidbody velocity.
    /// </summary>
    public void OnReleaseBall()
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            m_moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// Resets key components, including Rigidbody and CharacterController,
    /// and repositions the NPC to its designated spawn point.
    /// </summary>
    public void ResetComponents()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        controller.enabled = false;
        transform.position = npcCollectionSO.Players[NPCIndex].SpawnPoint;
        transform.rotation = Quaternion.identity;

        controller.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
    }
    
    /// <summary>
    /// Activates the CharacterController for the NPC, enabling movement control.
    /// </summary>
    public void EnableCharacterController() => controller.enabled = true;
    
    /// <summary>
    /// Deactivates the CharacterController for the NPC, disabling movement control.
    /// </summary>
    public void DisableCharacterController() => controller.enabled = false;
    
    /// <summary>
    /// Retrieves the ScriptableObject associated with the NPC, containing its data.
    /// </summary>
    /// <returns>The NPC's SO_Player ScriptableObject.</returns>
    public SO_Player GetNpcSO => npcSO;
}