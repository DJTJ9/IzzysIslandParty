using System.Collections.Generic;
using System.Linq;
using DependencyInjection;
using ImprovedTimers; // https://github.com/adammyhre/Unity-Dependency-Injection-Lite
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GoapAgent : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] float cooldownTimerDuration = 5f;
    
    [Header("Sensors")] 
    [SerializeField] Sensor chaseSensor;
    [SerializeField] Sensor attackSensor;
    
    [Header("Known Locations")] 
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform finishTransform;
    // [SerializeField] Transform restingPosition;
    // [SerializeField] Transform foodShack;
    // [SerializeField] Transform doorOnePosition;
    // [SerializeField] Transform doorTwoPosition;
    
    Rigidbody rb;
    GoapRigidbodyMovement rigidbodyMovement;
    
    CountdownTimer statsTimer;
    CountdownTimer moveCooldownTimer;
    
    private bool m_isOnCooldown;
    
    GameObject target;
    Vector3 destination;
    
    AgentGoal lastGoal;
    public AgentGoal currentGoal;
    public ActionPlan actionPlan;
    public AgentAction currentAction;
    
    public Dictionary<string, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;
    
    IGoapPlanner gPlanner;
    
    void Awake() {
        rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true;
        rigidbodyMovement = GetComponent<GoapRigidbodyMovement>();
        
        gPlanner = new GoapPlanner();
    }

    void Start() {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    void SetupBeliefs() {
        beliefs = new Dictionary<string, AgentBelief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);
        
        factory.AddBelief("Nothing", () => false);

        // factory.AddBelief("AgentHealthLow", () => health < 30);
        // factory.AddBelief("AgentIsHealthy", () => health >= 50);
        // factory.AddBelief("AgentStaminaLow", () => stamina < 10);
        // factory.AddBelief("AgentIsRested", () => stamina >= 50);
        
        factory.AddBelief("FindNextDestination", () => false);
        factory.AddBelief("CooldownComplete", () => !m_isOnCooldown);
        
        // factory.AddLocationBelief("AgentAtDoorOne", 3f, doorOnePosition);
        // factory.AddLocationBelief("AgentAtDoorTwo", 3f, doorTwoPosition);
        // factory.AddLocationBelief("AgentAtRestingPosition", 3f, restingPosition);
        // factory.AddLocationBelief("AgentAtFoodShack", 3f, foodShack);

        factory.AddLocationBelief("FinishInReach", 15f, finishTransform);
        factory.AddLocationBelief("PlayerClose", 15f, playerTransform);
        factory.AddLocationBelief("PlayerChasable", 30f, playerTransform);
        
        factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
        factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
        factory.AddBelief("AttackingPlayer", () => false); // Player can always be attacked, this will never become true
    }

    void SetupActions() {
        actions = new HashSet<AgentAction>();
        
        actions.Add(new AgentAction.Builder("Relax")
            .WithStrategy(new IdleStrategy(1))
            .AddEffect(beliefs["Nothing"])
            .Build());
        
        actions.Add(new AgentAction.Builder("IdleWhileOnCooldown")
            .WithStrategy(new IdleStrategy(moveCooldownTimer.CurrentTime))
            .AddEffect(beliefs["CooldownComplete"])
            .Build());

        actions.Add(new AgentAction.Builder("MoveToNextPosition")
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => finishTransform.position, cooldownTimerDuration))
            .AddEffect(beliefs["FindNextDestination"])
            .Build());
        
        // actions.Add(new AgentAction.Builder("Wander Around")
        //     .WithStrategy(new WanderStrategy(navMeshAgent, 10))
        //     .AddEffect(beliefs["AgentMoving"])
        //     .Build());

        actions.Add(new AgentAction.Builder("ChasePlayer")
            .WithStrategy(new MoveStrategy(rigidbodyMovement ,() => playerTransform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs["PlayerChasable"])
            .AddEffect(beliefs["AttackingPlayer"])
            .Build());

        actions.Add(new AgentAction.Builder("AttackPlayer")
            .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => playerTransform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs["PlayerClose"])
            .AddEffect(beliefs["AttackingPlayer"])
            .Build());
        
                #region Examples
        // actions.Add(new AgentAction.Builder("MoveToEatingPosition")
        //     .WithStrategy(new MoveStrategy(navMeshAgent, () => foodShack.position))
        //     .AddEffect(beliefs["AgentAtFoodShack"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("Eat")
        //     .WithStrategy(new IdleStrategy(5))  // Later replace with a Command
        //     .AddPrecondition(beliefs["AgentAtFoodShack"])
        //     .AddEffect(beliefs["AgentIsHealthy"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("MoveToDoorOne")
        //     .WithStrategy(new MoveStrategy(navMeshAgent, () => doorOnePosition.position))
        //     .AddEffect(beliefs["AgentAtDoorOne"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("MoveToDoorTwo")
        //     .WithStrategy(new MoveStrategy(navMeshAgent, () => doorTwoPosition.position))
        //     .AddEffect(beliefs["AgentAtDoorTwo"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("MoveFromDoorOneToRestArea")
        //     .WithCost(2)
        //     .WithStrategy(new MoveStrategy(navMeshAgent, () => restingPosition.position))
        //     .AddPrecondition(beliefs["AgentAtDoorOne"])
        //     .AddEffect(beliefs["AgentAtRestingPosition"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("MoveFromDoorTwoRestArea")
        //     .WithStrategy(new MoveStrategy(navMeshAgent, () => restingPosition.position))
        //     .AddPrecondition(beliefs["AgentAtDoorTwo"])
        //     .AddEffect(beliefs["AgentAtRestingPosition"])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder("Rest")
        //     .WithStrategy(new IdleStrategy(5))
        //     .AddPrecondition(beliefs["AgentAtRestingPosition"])
        //     .AddEffect(beliefs["AgentIsRested"])
        //     .Build());
        #endregion
    }

    void SetupGoals() {
        goals = new HashSet<AgentGoal>();
        
        goals.Add(new AgentGoal.Builder("Chill Out")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Nothing"])
            .Build());
        //
        // goals.Add(new AgentGoal.Builder("Wander")
        //     .WithPriority(1)
        //     .WithDesiredEffect(beliefs["AgentMoving"])
        //     .Build());

        goals.Add(new AgentGoal.Builder("ComingCloserToFinish")
            .WithPriority(20)
            .WithDesiredEffect(beliefs["FindNextDestination"])
            .Build());
        
        goals.Add(new AgentGoal.Builder("SeekAndDestroy")
            .WithPriority(50)
            .WithDesiredEffect(beliefs["AttackingPlayer"])
            .Build());

        goals.Add(new AgentGoal.Builder("WaitForCooldown")
            .WithPriority(100)
            .WithDesiredEffect(beliefs["CooldownComplete"])
            .Build());
    }

    void SetupTimers() {
        statsTimer = new CountdownTimer(2f);
        statsTimer.OnTimerStop += () => {
            UpdateStats();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
            statsTimer.Start();
        };
        statsTimer.Start();
        
        moveCooldownTimer = new CountdownTimer(3f);
        moveCooldownTimer.OnTimerStart += () => m_isOnCooldown = true;
        moveCooldownTimer.OnTimerStop += () => m_isOnCooldown = false;
    }

    // TODO move to stats system
    void UpdateStats() {
        // stamina += InRangeOf(restingPosition.position, 3f) ? 20 : -10;
        // health += InRangeOf(foodShack.position, 3f) ? 20 : -5;
        // stamina = Mathf.Clamp(stamina, 0, 100);
        // health = Mathf.Clamp(health, 0, 100);
    }
    
    bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) < range;
    
    // void OnEnable() => chaseSensor.OnTargetChanged += HandleTargetChanged;
    // void OnDisable() => chaseSensor.OnTargetChanged -= HandleTargetChanged;
    //
    // void HandleTargetChanged() {
    //     Debug.Log("Target changed, clearing current action and goal");
    //     // Force the planner to re-evaluate the plan
    //     currentAction = null;
    //     currentGoal = null;
    // }

    void Update() {
        statsTimer.Tick(Time.deltaTime);
        
        // Update the plan and current action if there is one
        if (currentAction == null) {
            Debug.Log("Calculating any potential new plan");
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0) {
                // navMeshAgent.ResetPath();

                currentGoal = actionPlan.AgentGoal;
                Debug.Log($"Goal: {currentGoal.Name} with {actionPlan.Actions.Count} actions in plan");
                currentAction = actionPlan.Actions.Pop();
                Debug.Log($"Popped action: {currentAction.Name}");
                // Verify all precondition effects are true
                if (currentAction.Preconditions.All(b => b.Evaluate())) {
                    currentAction.Start();
                } else {
                    Debug.Log("Preconditions not met, clearing current action and goal");
                    currentAction = null;
                    currentGoal = null;
                }
            }
        }

        // If we have a current action, execute it
        if (actionPlan != null && currentAction != null) {
            currentAction.Update(Time.deltaTime);

            if (currentAction.Complete) {
                Debug.Log($"{currentAction.Name} complete");
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.Actions.Count == 0) {
                    Debug.Log("Plan complete");
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    void CalculatePlan() {
        var priorityLevel = currentGoal?.Priority ?? 0;
        
        HashSet<AgentGoal> goalsToCheck = goals;
        
        // If we have a current goal, we only want to check goals with higher priority
        if (currentGoal != null) {
            Debug.Log("Current goal exists, checking goals with higher priority");
            goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
        }
        
        var potentialPlan = gPlanner.Plan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null) {
            actionPlan = potentialPlan;
        }
    }
}