using UnityEngine;

namespace HurdleGame.Camera
{
    [RequireComponent(typeof(CharacterMover))]
    public class CameraMoverAddition : MonoBehaviour
    {
        private CharacterMover cameraMover;
        
        [SerializeField] private CharacterMover orientationCharacter;
        [SerializeField] private float individualMultiplierDeduction = 0.04f;

        private void Awake()
        {
            cameraMover = GetComponent<CharacterMover>();
        }
        private void FixedUpdate()
        {
            cameraMover.IndividualMultiplier = (orientationCharacter.IndividualMultiplier) - individualMultiplierDeduction;
        }
    }
}
