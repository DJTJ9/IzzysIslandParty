using UnityEngine;

public class FilterGameEventListenerInt : GameEventListenerInt
{
    public int TriggerValue;

    public override void OnEventRaised(int value)
    {
        // If value not expected value
        if (value != TriggerValue) return;
    }
}
