using System;
using System.Collections.Generic;
using System.Linq;
using ImprovedTimers;
using UnityEngine;

public class GoapAgent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_playerAttackRange = 15f;

    [SerializeField] private float m_checkPointDetectionRange = 30f;
    [SerializeField] private float cooldownTimerDuration = 5f;

    [Header("Sensors")]
    [SerializeField] private Sensor chaseSensor;

    [SerializeField] private Sensor attackSensor;

    [Header("Known Locations")]
    // [SerializeField] private Transform player1Transform;
    // [SerializeField] private Transform player2Transform;
    // [SerializeField] private Transform player3Transform;
    private Transform finishTransform;

    private Transform checkPoint1;
    private Transform checkPoint2;
    private Transform checkPoint3;
    private Transform checkPoint4;
    private Transform checkPoint5;
    private Transform checkPoint6;
    private Transform checkPoint7;
    private Transform checkPoint8;
    private Transform checkPoint9;
    private Transform checkPoint10;

    private const int MAX_PLAYERS = 3;
    private const int MAX_CHECKPOINTS = 30;

    private GoapRigidbodyMovement rigidbodyMovement;

    private CountdownTimer statsTimer;
    private CountdownTimer moveCooldownTimer;

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

    private void Awake()
    {
        // rb = GetComponent<Rigidbody>();
        // rb.freezeRotation = true;
        rigidbodyMovement = GetComponent<GoapRigidbodyMovement>();

        gPlanner = new GoapPlanner();

        GetTransformsFromLocationService();
    }

    private void Start()
    {
        SetupTimers();
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupBeliefs()
    {
        beliefs = new Dictionary<Beliefs, AgentBelief>();
        var factory = new BeliefFactory(this, beliefs);

        CreatePlayerBeliefs(factory);
        // CreateCheckPointBeliefs(factory);

        factory.AddBelief(Beliefs.Nothing, () => false);

        factory.AddBelief(Beliefs.FindNextPosition, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint1, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint2, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint3, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint4, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint5, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint6, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint7, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint8, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint9, () => false);
        factory.AddBelief(Beliefs.ReachCheckPoint10, () => false);

        factory.AddBelief(Beliefs.WinGame, () => false);
        // factory.AddBelief("CooldownComplete", () => !m_isOnCooldown);

        factory.AddLocationBelief(Beliefs.FinishInReach, 50f, finishTransform);
        factory.AddLocationBelief(Beliefs.CheckPoint1InReach, 50f, checkPoint1);
        factory.AddLocationBelief(Beliefs.CheckPoint2InReach, 50f, checkPoint2);
        factory.AddLocationBelief(Beliefs.CheckPoint3InReach, 50f, checkPoint3);
        factory.AddLocationBelief(Beliefs.CheckPoint4InReach, 50f, checkPoint4);
        factory.AddLocationBelief(Beliefs.CheckPoint5InReach, 50f, checkPoint5);
        factory.AddLocationBelief(Beliefs.CheckPoint6InReach, 50f, checkPoint6);
        factory.AddLocationBelief(Beliefs.CheckPoint7InReach, 50f, checkPoint7);
        factory.AddLocationBelief(Beliefs.CheckPoint8InReach, 50f, checkPoint8);
        factory.AddLocationBelief(Beliefs.CheckPoint9InReach, 50f, checkPoint9);
        factory.AddLocationBelief(Beliefs.CheckPoint10InReach, 50f, checkPoint10);

        // factory.AddLocationBelief(Beliefs.Player1Close, 15f, player1Transform);
        // factory.AddLocationBelief(Beliefs.Player2Close, 15f, player2Transform);
        // factory.AddLocationBelief(Beliefs.Player3Close, 15f, player3Transform);

        // factory.AddSensorBelief("PlayerInChaseRange", chaseSensor);
        // factory.AddSensorBelief("PlayerInAttackRange", attackSensor);
        factory.AddBelief(Beliefs.AttackingPlayer1, () => false);
        factory.AddBelief(Beliefs.AttackingPlayer2, () => false);
        factory.AddBelief(Beliefs.AttackingPlayer3, () => false);
    }

    private void CreatePlayerBeliefs(BeliefFactory factory)
    {
        var playerTransforms = LocationService.Instance.PlayerTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));

        for (var i = 0; i < playerTransforms.Count; i++)
        {
            factory.AddLocationBelief(beliefValues[i], m_playerAttackRange, playerTransforms[$"Player{i + 1}"]);
        }
    }

    private void CreateCheckPointBeliefs(BeliefFactory factory)
    {
        var checkPointTransforms = LocationService.Instance.CheckPointTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));

        for (var i = 0; i < checkPointTransforms.Count; i++)
        {
            factory.AddBelief(beliefValues[i + MAX_PLAYERS + MAX_PLAYERS], () => false);
            factory.AddLocationBelief(beliefValues[i + MAX_PLAYERS + MAX_PLAYERS + MAX_CHECKPOINTS], m_checkPointDetectionRange, checkPointTransforms[$"CheckPoint{i + 1}"]);
        }
    }

    private void SetupActions()
    {
        actions = new HashSet<AgentAction>();

        CreatePlayerActions(actions);
        // CreateCheckPointActions(actions);

        actions.Add(new AgentAction.Builder(Actions.Relax)
            .WithStrategy(new IdleStrategy(1))
            .AddEffect(beliefs[Beliefs.Nothing])
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

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint4)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint4.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint4InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint4])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint5)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint5.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint5InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint5])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint6)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint6.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint6InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint6])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint7)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint7.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint7InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint7])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint8)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint8.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint8InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint8])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint9)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint9.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint9InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint9])
            .Build());

        actions.Add(new AgentAction.Builder(Actions.GoForCheckPoint10)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => checkPoint10.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.CheckPoint10InReach])
            .AddEffect(beliefs[Beliefs.ReachCheckPoint10])
            .Build());


        actions.Add(new AgentAction.Builder(Actions.GoForFinish)
            .WithStrategy(new AimForNextPositionStrategy(rigidbodyMovement, () => finishTransform.position, cooldownTimerDuration))
            .AddPrecondition(beliefs[Beliefs.FinishInReach])
            .AddEffect(beliefs[Beliefs.WinGame])
            .Build());


        #region Examples

        // actions.Add(new AgentAction.Builder("Wander Around")
        //     .WithStrategy(new WanderStrategy(navMeshAgent, 10))
        //     .AddEffect(beliefs["AgentMoving"])
        //     .Build());

        // actions.Add(new AgentAction.Builder("ChasePlayer")
        //     .WithStrategy(new MoveStrategy(rigidbodyMovement ,() => player1Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs["PlayerChasable"])
        //     .AddEffect(beliefs["AttackingPlayer1"])
        //     .Build());

        // actions.Add(new AgentAction.Builder(Actions.AttackPlayer1)
        //     .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player1Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs[Beliefs.Player1Close])
        //     .AddEffect(beliefs[Beliefs.AttackingPlayer1])
        //     .Build());
        //
        // actions.Add(new AgentAction.Builder(Actions.AttackPlayer2)
        //     .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player2Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs[Beliefs.Player2Close])
        //     .AddEffect(beliefs[Beliefs.AttackingPlayer2])
        //     .Build());

        // actions.Add(new AgentAction.Builder(Actions.AttackPlayer3)
        //     .WithStrategy(new AttackStrategy(rigidbodyMovement ,() => player3Transform.position, cooldownTimerDuration))
        //     .AddPrecondition(beliefs[Beliefs.Player3Close])
        //     .AddEffect(beliefs[Beliefs.AttackingPlayer3])
        //     .Build());

        #endregion
    }

    private void CreatePlayerActions(HashSet<AgentAction> _actions)
    {
        var playerTransforms = LocationService.Instance.PlayerTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));
        var actionValues = (Actions[])Enum.GetValues(typeof(Actions));

        for (var i = 0; i < playerTransforms.Count; ++i)
        {
            _actions.Add(new AgentAction.Builder(actionValues[i])
                .WithStrategy(new AttackStrategy(rigidbodyMovement, () => playerTransforms[$"Player{i}"].position, cooldownTimerDuration))
                .AddPrecondition(beliefs[beliefValues[i]])
                .AddEffect(beliefs[beliefValues[i + MAX_PLAYERS]])
                .Build());
        }
    }

    private void CreateCheckPointActions(HashSet<AgentAction> _actions)
    {
        var checkPointTransforms = LocationService.Instance.CheckPointTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));
        var actionValues = (Actions[])Enum.GetValues(typeof(Actions));

        for (var i = 0; i < checkPointTransforms.Count - 1; ++i)
        {
            _actions.Add(new AgentAction.Builder(actionValues[i + MAX_PLAYERS])
                .WithStrategy(new AttackStrategy(rigidbodyMovement, () => checkPointTransforms[$"CheckPoint{i + 1}"].position, cooldownTimerDuration))
                .AddPrecondition(beliefs[beliefValues[i + MAX_PLAYERS + MAX_PLAYERS + MAX_CHECKPOINTS]])
                .AddEffect(beliefs[beliefValues[i + MAX_PLAYERS + MAX_PLAYERS]])
                .Build());
        }
    }

    private void CreatePlayerGoals(HashSet<AgentGoal> _goals)
    {
        var playerTransforms = LocationService.Instance.PlayerTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));
        var goalValues = (Goals[])Enum.GetValues(typeof(Goals));

        for (var i = 0; i < playerTransforms.Count; ++i)
        {
            _goals.Add(new AgentGoal.Builder(goalValues[i])
                .WithPriority(500)
                .WithDesiredEffect(beliefs[beliefValues[i + 3]])
                .Build());
        }
    }

    private void CreateCheckPointGoals(HashSet<AgentGoal> _goals)
    {
        var CheckPointTransforms = LocationService.Instance.CheckPointTransforms;
        var beliefValues = (Beliefs[])Enum.GetValues(typeof(Beliefs));
        var goalValues = (Goals[])Enum.GetValues(typeof(Goals));

        var priority = 100;

        for (var i = 0; i < CheckPointTransforms.Count; ++i)
        {
            _goals.Add(new AgentGoal.Builder(goalValues[i + MAX_PLAYERS])
                .WithPriority(priority)
                .WithDesiredEffect(beliefs[beliefValues[i + MAX_PLAYERS + MAX_PLAYERS]])
                .Build());

            priority += 10;
        }
    }

    private void SetupGoals()
    {
        goals = new HashSet<AgentGoal>();

        CreatePlayerGoals(goals);
        // CreateCheckPointGoals(goals);

        goals.Add(new AgentGoal.Builder(Goals.ChillOut)
            .WithPriority(1)
            .WithDesiredEffect(beliefs[Beliefs.Nothing])
            .Build());

        // goals.Add(new AgentGoal.Builder(Goals.AttackPlayer1)
        //     .WithPriority(250)
        //     .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer1])
        //     .Build());
        //
        // goals.Add(new AgentGoal.Builder(Goals.AttackPlayer2)
        //     .WithPriority(250)
        //     .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer2])
        //     .Build());
        //
        // goals.Add(new AgentGoal.Builder(Goals.AttackPlayer3)
        //     .WithPriority(250)
        //     .WithDesiredEffect(beliefs[Beliefs.AttackingPlayer3])
        //     .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint1)
            .WithPriority(100)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint1])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint2)
            .WithPriority(110)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint2])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint3)
            .WithPriority(120)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint3])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint4)
            .WithPriority(130)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint4])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint5)
            .WithPriority(140)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint5])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint6)
            .WithPriority(150)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint6])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint7)
            .WithPriority(160)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint7])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint8)
            .WithPriority(170)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint8])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint9)
            .WithPriority(180)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint9])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForCheckPoint10)
            .WithPriority(190)
            .WithDesiredEffect(beliefs[Beliefs.ReachCheckPoint10])
            .Build());

        goals.Add(new AgentGoal.Builder(Goals.GoForFinish)
            .WithPriority(1000)
            .WithDesiredEffect(beliefs[Beliefs.WinGame])
            .Build());
    }

    private void SetupTimers()
    {
        statsTimer = new CountdownTimer(1f);
        statsTimer.OnTimerStop += () =>
        {
            SetupBeliefs();
            SetupActions();
            SetupGoals();
            statsTimer.Start();
        };
        statsTimer.Start();
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

    private void Update()
    {
        statsTimer.Tick(Time.deltaTime);

        if (currentAction == null)
        {
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0)
            {
                currentGoal = actionPlan.AgentGoal;
                currentAction = actionPlan.Actions.Pop();
                if (currentAction.Preconditions.All(b => b.Evaluate()))
                {
                    currentAction.Start();
                }
                else
                {
                    currentAction = null;
                    currentGoal = null;
                }
            }
        }

        if (actionPlan != null && currentAction != null)
        {
            currentAction.Update(Time.deltaTime);

            if (currentAction.Complete)
            {
                currentAction.Stop();
                currentAction = null;

                if (actionPlan.Actions.Count == 0)
                {
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    private void CalculatePlan()
    {
        var priorityLevel = currentGoal?.Priority ?? 0;

        HashSet<AgentGoal> goalsToCheck = goals;

        if (currentGoal != null)
        {
            goalsToCheck = new HashSet<AgentGoal>(goals.Where(g => g.Priority > priorityLevel));
        }

        var potentialPlan = gPlanner.Plan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }

    private void GetTransformsFromLocationService()
    {
        // player1Transform = LocationService.Instance.GetTransform("Player1");
        // player2Transform = LocationService.Instance.GetTransform("Player2");
        // player3Transform = LocationService.Instance.GetTransform("Player3");
        finishTransform = LocationService.Instance.GetTransform("Finish");
        checkPoint1 = LocationService.Instance.GetTransform("CheckPoint1");
        checkPoint2 = LocationService.Instance.GetTransform("CheckPoint2");
        checkPoint3 = LocationService.Instance.GetTransform("CheckPoint3");
        checkPoint4 = LocationService.Instance.GetTransform("CheckPoint4");
        checkPoint5 = LocationService.Instance.GetTransform("CheckPoint5");
        checkPoint6 = LocationService.Instance.GetTransform("CheckPoint6");
        checkPoint7 = LocationService.Instance.GetTransform("CheckPoint7");
        checkPoint8 = LocationService.Instance.GetTransform("CheckPoint8");
        checkPoint9 = LocationService.Instance.GetTransform("CheckPoint9");
        checkPoint10 = LocationService.Instance.GetTransform("CheckPoint10");
    }
}