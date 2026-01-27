using UnityEngine;

namespace HurdleGame
{
    public class ObstacleBehaviour : MonoBehaviour
    {
        [SerializeField] private CustomTriggerBehaviour obstacleTrigger;
        [SerializeField] private FloatReference speedPenalty;

        private bool hitObstacle = false;

        private void Start()
        {
            obstacleTrigger.EnteredTriggerAction += OnObstacleHit;
        }

        private void OnObstacleHit(Collider _obj)
        {
            if (_obj.TryGetComponent<CharacterMover>(out var character) && !hitObstacle)
            {
                hitObstacle = true;
                character.HitObstacle();
            }
        }
    }
}