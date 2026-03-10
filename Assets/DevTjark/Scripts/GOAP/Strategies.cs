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
    
    public bool CanPerform => true;
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
    
    public void Update(float _deltaTime)
    {
        shootCooldownTimer.Tick(_deltaTime);
    }
    public void Stop()
    {
        shootCooldownTimer.Stop();
    }
}

public class AimForNextPositionStrategy : IActionStrategy
{
    private readonly NavMeshAgent agent;
    private readonly GoapRigidbodyMovement rigidbodyMovement;
    readonly Func<Vector3> finish;
    private CountdownTimer shootCooldownTimer;
    private readonly float shootCooldownTimerDuration;

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

        rigidbodyMovement.Shoot(finish());
        shootCooldownTimer.Start();
    }

    public void Update(float _deltaTime)
    {
        shootCooldownTimer.Tick(_deltaTime);
    }
    
    public void Stop()
    {
        shootCooldownTimer.Stop();
    }
}

public class IdleStrategy : IActionStrategy
{
    public bool CanPerform => true;
    public bool Complete   { get; private set; }

    readonly CountdownTimer timer;

    public IdleStrategy(float duration)
    {
        timer = new CountdownTimer(duration);
        timer.OnTimerStart += () => Complete = false;
        timer.OnTimerStop += () => Complete = true;
    }

    public void Start()
    {
        timer.Start();
    }

    public void Update(float _deltaTime)
    {
        timer.Tick(_deltaTime);
    }

    public void Stop()
    {
        timer.Stop();
    }
}