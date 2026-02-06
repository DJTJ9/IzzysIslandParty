using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerInt : MonoBehaviour
{
    public GameEventInt Event;

    public UnityEvent<int> Response;

    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(int _value)
    {
        Response.Invoke(_value);
    }
}
