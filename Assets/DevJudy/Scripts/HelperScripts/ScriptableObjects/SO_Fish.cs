using System.Collections.Generic;
using enums;
using FishingGame.QuickTimeEvents;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Fishing/Create Fish")]
    public class SO_Fish : ScriptableObject
    {
        [field: SerializeField] public string FishName { get; private set; }

        [field: SerializeField] public EFish FishType { get; private set; }

        [field: SerializeField] public int Probability { get; private set; }

        [field: SerializeField] public int Points { get; private set; }

        [SerializeField] public List<EQuickTimeEvent> CatchEvents;
    }
}