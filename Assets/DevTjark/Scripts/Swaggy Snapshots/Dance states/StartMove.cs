using UnityEngine;

public class StartMove : StateMachineBehaviour
{
    [SerializeField] private float duration = 3f;

    [SerializeField] private DanceMoveTriggersSO danceMoveTriggersSO;
    
    private float timer;
    private bool hasTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        hasTriggered = false;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hasTriggered) return;
        
        timer += Time.deltaTime;
        
        if (timer >= duration)
        {
            hasTriggered = true;
            animator.SetTrigger(danceMoveTriggersSO.GetNextDanceMove());
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
    }
}
