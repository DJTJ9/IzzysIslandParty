using UnityEngine;

public class DanceMoveSwitcher : StateMachineBehaviour
{
    // [Header("Settings")]
    // [SerializeField]
    // private float m_duration = 3f;

    [SerializeField]
    private DanceMoveTriggersSO danceMoveTriggersSO;

    [SerializeField]
    private GameEvent onDanceMoveChanged;

    private Animator m_animator;
    // private float timer;
    // private bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_animator = animator;
        // timer = 0;
        // hasTriggered = false;
    }

    // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     if (hasTriggered) return;
    //     
    //     timer += Time.deltaTime;
    //     
    //     if (timer >= m_duration) OnDanceMoveChange();
    // }

    public void OnDanceMoveChange()
    {
        // hasTriggered = true;
        m_animator.SetTrigger(danceMoveTriggersSO.GetRandomDanceMove());
    }

    // public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     timer = 0;
    // }
}