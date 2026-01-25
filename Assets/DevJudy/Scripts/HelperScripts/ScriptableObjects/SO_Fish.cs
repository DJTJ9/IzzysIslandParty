using System.Collections.Generic;
using enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/FishingFrenzy/Fish")]
    public class SO_Fish : ScriptableObject
    {
        [Header("Display variables: ")]
        [field: SerializeField] public string FishName { get; private set; }

        [field: SerializeField] public EFish FishType { get; private set; }

        [field: SerializeField] public Vector2 SizeRange { get; private set; }
        [field: SerializeField] public Vector2 WeightRange { get; private set; }
        
        [Header("Logic variables: ")]
        [field: SerializeField] public int Probability { get; private set; }
        [field: SerializeField] public int Points { get; private set; }
        [field: SerializeField] public List<EQuickTimeEvent> CatchEvents{ get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        
        [HideInInspector] public GameObject PrefabReference;

        public decimal GetRandomFromRange(Vector2 _fishSizeRange)
        {
            decimal random = (decimal)Random.Range(_fishSizeRange.x,  _fishSizeRange.y);

            random = decimal.Round(random, 2);
            
            return random;
        }
    }
}