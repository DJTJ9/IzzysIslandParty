using System;
using ImprovedTimers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// TODO Migrate Strategies, Beliefs, Actions and Goals to Scriptable Objects and create Node Editor for them
public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Complete   { get; }

    void Start()
    {
        // noop
    }

    void Update(float _deltaTime)
    {
        // noop
    }

    void Stop()
    {
        // noop
    }
}

public class AttackStrategy : IActionStrategy
{
    public bool CanPerform => true; // Agent can always attack
    public bool Complete   { get; private set; }

    readonly CountdownTimer timer;

    public AttackStrategy()
    {
    }

    public void Start()
    {
        timer.Start();
    }

    public void Update(float _deltaTime) => timer.Tick(_deltaTime);
}

public class MoveStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete   => agent.remainingDistance <= 2f && !agent.pathPending;

    public MoveStrategy(NavMeshAgent agent, Func<Vector3> destination)
    {
        this.agent = agent;
        this.destination = destination;
    }

    public void Start() => agent.SetDestination(destination());
    public void Stop()  => agent.ResetPath();
}

public class AimForNextPositionStrategy : IActionStrategy
{
    private readonly NavMeshAgent agent;
    private readonly GoapRigidbodyMovement rigidbodyMovement;
    private CountdownTimer shootCooldownTimer;
    private readonly float shootRange = 15f;

    public bool CanPerform => !Complete;
    public bool Complete   { get; private set; }

    public AimForNextPositionStrategy(NavMeshAgent _agent, GoapRigidbodyMovement _rigidbodyMovement)
    {
        agent = _agent;
        rigidbodyMovement = _rigidbodyMovement;
    }

    public void Start()
    {
        shootCooldownTimer = new CountdownTimer(5f);
        shootCooldownTimer.OnTimerStart += () =>
        {
            Complete = false;
        };
        shootCooldownTimer.OnTimerStop += () => Complete = true;
        
        Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * shootRange);
        randomDirection.y = 0;

        rigidbodyMovement.Shoot(randomDirection + new Vector3(0, 3f, 0));
        shootCooldownTimer.Start();
        // if (NavMesh.SamplePosition(agent.transform.position + randomDirection, out var hit, shootRange, 1)) {
        // }
    }

    // public void Update(float _deltaTime)
    // {
    //     shootCooldownTimer.Tick(_deltaTime);
    // }
}

public class WanderStrategy : IActionStrategy
{
    readonly NavMeshAgent agent;
    readonly float wanderRadius;

    public bool CanPerform => !Complete;
    public bool Complete   => agent.remainingDistance <= 2f && !agent.pathPending;

    public WanderStrategy(NavMeshAgent agent, float wanderRadius)
    {
        this.agent = agent;
        this.wanderRadius = wanderRadius;
    }

    public void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomDirection = (UnityEngine.Random.insideUnitSphere * wanderRadius);
            randomDirection.y = 0;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(agent.transform.position + randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true; // Agent can always Idle
    public bool Complete   { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start()                 => timer.Start();
    public void Update(float _deltaTime) => timer.Tick(_deltaTime);
}