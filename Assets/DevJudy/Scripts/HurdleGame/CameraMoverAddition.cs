using UnityEngine;

namespace HurdleGame.Camera
{
    [RequireComponent(typeof(CharacterMover))]
    public class CameraMoverAddition : MonoBehaviour
    {
        private CharacterMover cameraMover;

        private CharacterMover orientationCharacter;
        [SerializeField] private float individualMultiplierDeduction = 0.04f;

        private void Awake()
        {
            cameraMover = GetComponent<CharacterMover>();
        }

        public void SetOrientationCharacter(CharacterMover _characterMover)
        {
            orientationCharacter = _characterMover;
        }

        private void FixedUpdate()
        {
            if (orientationCharacter != null)
                cameraMover.IndividualMultiplier = ((orientationCharacter.IndividualMultiplier) - individualMultiplierDeduction);
        }
    }
}