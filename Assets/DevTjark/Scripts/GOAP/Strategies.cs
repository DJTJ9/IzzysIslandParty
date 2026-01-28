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
    readonly GoapRigidbodyMovement rigidbodyMovement;
    private CountdownTimer shootCooldownTimer;
    private readonly float shootCooldownTimerDuration;
    readonly Func<Vector3> destination;
    
    public bool CanPerform => true; // Agent can always attack
    public bool Complete   { get; private set; }


    public AttackStrategy(GoapRigidbodyMovement _rigidbodyMovement, Func<Vector3> _destination, float _shootCooldownTimerDuration = 5f)
    {
        rigidbodyMovement = _rigidbodyMovement;
        destination = _destination;
        shootCooldownTimerDuration = _shootCooldownTimerDuration;
    }

    public void Start()
    {
        shootCooldownTimer = new CountdownTimer(shootCooldownTimerDuration);
        shootCooldownTimer.OnTimerStart += () => Complete = false;
        shootCooldownTimer.OnTimerStop += () => Complete = true;
        
        rigidbodyMovement.Shoot(destination() + new Vector3(0, 1f, 0));
        shootCooldownTimer.Start();
    }
    
    public void Stop()
    {
        shootCooldownTimer.Dispose();
    }
}

public class MoveStrategy : IActionStrategy
{
    readonly GoapRigidbodyMovement rigidbodyMovement;
    private CountdownTimer shootCooldownTimer;
    private readonly float shootCooldownTimerDuration;
    readonly Func<Vector3> destination;

    public bool CanPerform => !Complete;
    public bool Complete   { get; private set; }

    public MoveStrategy(GoapRigidbodyMovement _rigidbodyMovement, Func<Vector3> _destination, float _shootCooldownTimerDuration)
    {
        rigidbodyMovement = _rigidbodyMovement;
        destination = _destination;
        shootCooldownTimerDuration = _shootCooldownTimerDuration;
    }

    public void Start()
    {
        shootCooldownTimer = new CountdownTimer(shootCooldownTimerDuration);
        shootCooldownTimer.OnTimerStart += () => Complete = false;
        shootCooldownTimer.OnTimerStop += () => Complete = true;
        
        rigidbodyMovement.Shoot(destination());
        shootCooldownTimer.Start();
    }

    public void Stop()
    {
        shootCooldownTimer.Dispose();
    }
}

public class AimForNextPositionStrategy : IActionStrategy
{
    private readonly NavMeshAgent agent;
    private readonly GoapRigidbodyMovement rigidbodyMovement;
    readonly Func<Vector3> finish;
    private CountdownTimer shootCooldownTimer;
    private readonly float shootCooldownTimerDuration;
    // private readonly float shootRange = 15f;

    public bool CanPerform => !Complete;
    public bool Complete   { get; private set; }

    public AimForNextPositionStrategy(GoapRigidbodyMovement _rigidbodyMovement, Func<Vector3> _finish, float _shootCooldownTimerDuration)
    {
        rigidbodyMovement = _rigidbodyMovement;
        finish = _finish;
        shootCooldownTimerDuration = _shootCooldownTimerDuration;
    }

    public void Start()
    {
        shootCooldownTimer = new CountdownTimer(shootCooldownTimerDuration);
        shootCooldownTimer.OnTimerStart += () => Complete = false;
        shootCooldownTimer.OnTimerStop += () => Complete = true;
        
        Vector3 currentPosition = rigidbodyMovement.transform.position;
        Vector3 finishPosition = finish();
        float currentDistanceToFinish = Vector3.Distance(currentPosition, finishPosition);
        
        Vector3 randomDirection = (finishPosition - currentPosition).normalized + new Vector3(0f, 1f, 0);
        // float newDistanceToFinish;
        //
        // do
        // {
        //     randomDirection = (UnityEngine.Random.insideUnitSphere * shootRange);
        //     
        //     Vector3 potentialNewPosition = currentPosition + randomDirection;
        //     newDistanceToFinish = Vector3.Distance(potentialNewPosition, finishPosition);
        // } 
        // while (newDistanceToFinish >= currentDistanceToFinish);
        //
        // randomDirection.y = 1.5f;
        rigidbodyMovement.Shoot(finish());
        shootCooldownTimer.Start();
    }

    // public void Update(float _deltaTime)
    // {
    //     shootCooldownTimer.Tick(_deltaTime);
    // }
    
    public void Stop()
    {
        shootCooldownTimer.Dispose();
    }
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