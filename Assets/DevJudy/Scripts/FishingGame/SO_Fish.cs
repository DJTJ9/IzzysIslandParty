using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Fishing/Create Fish")]
public class So_Fish : ScriptableObject
{
    [field: SerializeField] public string FishName { get; private set; }

    [field: SerializeField] public int Probability { get; private set; }
    
    [field: SerializeField] public int Points { get; private set; }

    [field: SerializeField] public UnityEvent CatchTimeEvent;
}