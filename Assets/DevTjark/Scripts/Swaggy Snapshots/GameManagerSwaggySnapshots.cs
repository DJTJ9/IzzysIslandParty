using UnityEngine;
using ImprovedTimers;
using UnityEngine.Events;

public class GameManagerSwaggySnapshots : MonoBehaviour
{
    [SerializeField] private float roundTime = 15;
    
    [SerializeField] private UnityEvent onRoundEnd;
    
    private CountdownTimer roundTimer;
    
    private void Start()
    {
        roundTimer = new CountdownTimer(roundTime);
        roundTimer.OnTimerStop += OnRoundEnd;
        roundTimer.Start();
    }
    
    private void OnRoundEnd()
    {
        onRoundEnd.Invoke();
    }
}
