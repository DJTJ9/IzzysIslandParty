using UnityEngine;

using UnityEngine;

public class GateBehaviour : MonoBehaviour
{
    //[SerializeField] private UpdateUITimer uiTimer;
    
    [SerializeField] private CustomTriggerBehaviour middleCollider;
    [SerializeField] private CustomTriggerBehaviour leftCollider;
    [SerializeField] private CustomTriggerBehaviour rightCollider;

    [SerializeField] private FloatReference timeDeduction;
    
    private bool clearedGate = false;
    
    private void Awake()
    {
        middleCollider.EnteredTriggerAction += OnMiddleGateEnter;
        
        leftCollider.EnteredTriggerAction += OnSideGatesEnter;
        rightCollider.EnteredTriggerAction += OnSideGatesEnter;
    }
    
    private void OnMiddleGateEnter(Collider _other)
    {
        if (!clearedGate)
        {
            clearedGate = true;
            
            // Give visual feedback
        }
    }

    private void OnSideGatesEnter(Collider _other)
    {
        if (!clearedGate)
        {
            clearedGate = true;
            
            // Give visual feedback
            //uiTimer?.DeduceTime(timeDeduction.Value);
        }
    }
}