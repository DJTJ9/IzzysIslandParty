using UnityEngine;

namespace HurdleGame.Camera
{
    [RequireComponent(typeof(CharacterMover))]
    public class CameraMoverAddition : MonoBehaviour
    {
        private CharacterMover cameraMover;
        
        [SerializeField] private CharacterMover orientationCharacter;

        private void Awake()
        {
            cameraMover = GetComponent<CharacterMover>();
        }
        private void FixedUpdate()
        {
            cameraMover.IndividualMultiplier = (orientationCharacter.IndividualMultiplier) - 0.02f;
        }
    }
}
