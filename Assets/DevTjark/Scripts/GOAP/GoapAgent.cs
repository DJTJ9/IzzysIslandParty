using System.Collections.Generic;
using System.Linq;
using ImprovedTimers;
using UnityEngine;

public class GoapAgent : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private float cooldownTimerDuration = 5f;
    
    [Header("Sensors")] 
    [SerializeField] private Sensor chaseSensor;
    [SerializeField] private Sensor attackSensor;
    
    [Header("Known Locations")] 
    [SerializeField] private Transform player1Transform;
    [SerializeField] private Transform player2Transform;
    // [SerializeField] private Transform player3Transform;
    [SerializeField] private Transform finishTransform;
    [SerializeField] private Transform checkPoint1;
    [SerializeField] private Transform checkPoint2;
    [SerializeField] private Transform checkPoint3;
    
    private Rigidbody rb;
    private GoapRigidbodyMovement rigidbodyMovement;

    private CountdownTimer statsTimer;
    private CountdownTimer moveCooldownTimer;
    
    // private bool m_isOnCooldown;
    
    private GameObject target;
    private Vector3 destination;
    
    private AgentGoal lastGoal;
    public AgentGoal currentGoal;
    public ActionPlan actionPlan;
    public AgentAction currentAction;
    
    public Dictionary<Beliefs, AgentBelief> beliefs;
    public HashSet<AgentAction> actions;
    public HashSet<AgentGoal> goals;
    
    private IGoapPlanner gPlanner;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true;
        rigidbodyMovement = GetComponent<GoapRigidbodyMovement>();
        
        gPlanner = new GoapPlanner();

        player1Transform = LocationService.Instance.GetTransform("Player1");
        player2Transform = LocationService.Instance.GetTransform("Player2");
        // player3Transform = LocationService.Instance.GetTransform("Player3");
        finishTransform = LocationService.Instance.GetTransform("Finish");
        checkPoint1 = LocationService.Instance.GetTransform("CheckPoint1");
        checkPoint2 = LocationService.Instance.GetTransform("CheckPoint2");
        checkPoint3 = LocationService.Instance.GetTransform("CheckPoint3");
    }

    private void Start() {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupBeliefs() {
        beliefs = new Dictionary<Beliefs, AgentBelief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);
        
        factory.AddBelief(Beliefs.Nothing, () => false);

        // factory.AddBelief("AgentHealthLow", () => health < 30);
        // factory.AddBelief("AgentIsHealthy", () => health >= 50);
        // factory.AddBelief("AgentStaminaLow", () => stamina < 10);
        // factory.AddBelief("AgentIsRested", () => stamina >= 50);
        
        factory.AddBelief(Beliefs.FindNextPosition, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint1, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint2, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint3, () => false);
        factory.AddBelief(Beliefs.WinGame, () => false);
        // factory.AddBelief("CooldownComplete", () => !m_isOnCooldown);
        
        // factory.AddLocationBelief("AgentAtDoorOne", 3f, doorOnePosition);
        // factory.AddLocationBelief("AgentAtDoorTwo", 3f, doorTwoPosition);
        // factory.AddLocationBelief("AgentAtRestingPosition", 3f, restingPosition);
        // factory.AddLocationBelief("AgentAtFoodShack", 3f, foodShack);

        factory.AddLocationBelief(Beliefs.FinishInReach, 30f, finishTransform);
        factory.AddLocationBelief(Beliefs.CheckPoint1InReach, 30f, checkPoint1);
        factory.AddLocationBelief(Beliefs.CheckPoint2InReach, 30f, checkPoint2);
        factory.AddLocationBelief(Beliefs.CheckPoint3InReach, 30f, checkPoint3);
        
        factory.AddLocationBelief(Beliefs.Player1Close, 15f, player1Transform);
        factory.AddLocationBelief(Beliefs.Player2Close, 15f, player2Transform);
        // factory.AddLocationBelief(Beliefs.Player3Close, 15f, player3Transform);
        // factory.AddLocationBelief("PlayerChasable", 30f, player1Transform);
        
        // factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
        // factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
        factory.AddBelief(Beliefs.AttackingPlayer1, () => false);
        factory.AddBelief(Beliefs.AttackingPlayer2, () => false);
        factory.AddBelief(Beliefs.AttackingPlayer3, () => false);
    }

    private void SetupActions() {
        actions = new HashSet<AgentAction>();
        
        actions.Add(new AgentAction.Builder(Actions.Relax)
            .WithStrategy(new IdleStrategy(1))
            .AddEffect(beliefs[Beliefs.Nothing])
            .Build());
        
        // actions.Add(new AgentAction.Builder("IdleWhileOnCooldown")
        //     .WithStrategy(new IdleStrategy(moveCooldownTimer.CurrentTime))
        //     .AddEffect(beliefs["CooldownComplete"])
        //     .Build());

        actions.Add(new AgentAction.Builder(Actions.MoveToNextPosition)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => finishTransform.position, cooldownTimerDuration))
            .AddEffect(beliefs[Beliefs.FindNextPosition])
            .Build());
        
        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint1)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint1.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint1InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint1])
            .Build());
        
        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint2)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint2.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint2InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint2])
            .Build());
        
        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint3)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint3.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint3InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint3])
            .Build());
        
        actions.Add(new AgentAction.Builder(Actions.GoForFinish)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => finishTransform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.FinishInReach])
            .AddEffect(beliefs[Beliefs.WinGame])
            .Build());
        
        // actions.Add(new AgentAction.Builder("Wander Around")
        //     .WithStrategy(new WanderStrategy(navMeshAgent, 10))
        //     .AddEffect(beliefs["AgentMoving"])
        //     .Build());

        // actions.Add(new AgentAction.Builder("ChasePlayer")
        //     .WithStrategy(new MoveStrategy(rigidbodyMovement ,() => player1Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs["PlayerChasable"])
        //     .AddEffect(beliefs["AttackingPlayer1"])
        //     .Build());

        actions.Add(new AgentAction.Builder(Actions.AttackPlayer1)
            .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player1Transform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.Player1Close])
            .AddEffect(beliefs[Beliefs.AttackingPlayer1])
            .Build());
        
        actions.Add(new AgentAction.Builder(Actions.AttackPlayer2)
            .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player2Transform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.Player2Close])
            .AddEffect(beliefs[Beliefs.AttackingPlayer2])
            .Build());
        
        // actions.Add(new AgentAction.Builder(Actions.AttackPlayer3)
        //     .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player3Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs[Beliefs.Player3Close])
        //     .AddEffect(beliefs[Beliefs.AttackingPlayer3])
        //     .Build());
        
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

    private void SetupGoals() {
        goals = new HashSet<AgentGoal>();
        
        goals.Add(new AgentGoal.Builder(Goals.ChillOut)
            .WithPriority(1)
            .WithDesiredEffect(beliefs[Beliefs.Nothing])
            .Build());
        //
        // goals.Add(new AgentGoal.Builder("Wander")
        //     .WithPriority(1)
        //     .WithDesiredEffect(beliefs["AgentMoving"])
        //     .Build());

        goals.Add(new AgentGoal.Builder(Goals.ComingCloserToFinish)
            .WithPriority(20)
            .WithDesiredEffect(beliefs[Beliefs.FindNextPosition])
            .Build());
        
        goals.Add(new AgentGoal.Builder(Goals.AttackPlayer1)
            .WithPriority(250)
            .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer1])
            .Build());
        
        goals.Add(new AgentGoal.Builder(Goals.AttackPlayer2)
            .WithPriority(250)
            .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer2])
            .Build());
        
        // goals.Add(new AgentGoal.Builder(Goals.AttackPlayer3)
        //     .WithPriority(250)
        //     .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer3])
        //     .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint1)
            .WithPriority(100)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint1])
            .Build());
        
        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint2)
            .WithPriority(200)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint2])
            .Build());
        
        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint3)
            .WithPriority(300)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint3])
            .Build());
        
        goals.Add(new AgentGoal.Builder(Goals.GoForFinish)
            .WithPriority(1000)
            .WithDesiredEffect(beliefs[Beliefs.WinGame])
            .Build());
    }

    private void SetupTimers() {
        statsTimer = new CountdownTimer(1f);
        statsTimer.OnTimerStop += () => {
            UpdateStats();
            SetupBeliefs();
            SetupActions();
            SetupGoals();
            statsTimer.Start();
        };
        statsTimer.Start();
        
        // moveCooldownTimer = new CountdownTimer(3f);
        // moveCooldownTimer.OnTimerStart += () => m_isOnCooldown = true;
        // moveCooldownTimer.OnTimerStop += () => m_isOnCooldown = false;
    }

    // TODO move to stats system
    private void UpdateStats() {
        // stamina += InRangeOf(restingPosition.position, 3f) ? 20 : -10;
        // health += InRangeOf(foodShack.position, 3f) ? 20 : -5;
        // stamina = Mathf.Clamp(stamina, 0, 100);
        // health = Mathf.Clamp(health, 0, 100);
    }
    
    private bool InRangeOf(Vector3 pos, float range) => Vector3.Distance(transform.position, pos) < range;
    
    // void OnEnable() => chaseSensor.OnTargetChanged += HandleTargetChanged;
    // void OnDisable() => chaseSensor.OnTargetChanged -= HandleTargetChanged;
    //
    // void HandleTargetChanged() {
    //     Debug.Log("Target changed, clearing current action and goal");
    //     // Force the planner to re-evaluate the plan
    //     currentAction = null;
    //     currentGoal = null;
    // }

    private void Update() {
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

    private void CalculatePlan() {
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