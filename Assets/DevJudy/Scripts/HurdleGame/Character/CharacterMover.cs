using Audio;
using enums;
using Helper;
using Juice;
using UnityEngine;
using UnityEngine.Events;

namespace HurdleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        private const float moveDirMultiplier = 100f;
        
        private Rigidbody rb;
        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private UnityEvent OnHitObstacleEvents;
        [SerializeField] private FloatReference moveSpeed;
        [SerializeField] private FloatReference obstacleHitDeduction;
        
        [Header("Audio: ")]
        [SerializeField] private Vector2 hitVolumeRange = new Vector2(1f, 1f);
        [SerializeField] private Vector2 hitPitchRange = new Vector2(1f, 1f);
        
        private float individualMultiplier = 1f;
        public float IndividualMultiplier
        {
            get => individualMultiplier;
            set => individualMultiplier = value;
        }

        private bool canMove = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
        }

        public void CanMove()
        {
            canMove = true;
        }

        public void CantMove()
        {
            canMove = false;
            rb.linearVelocity = Vector3.zero;
        }

        public void OnHitObstacle()
        {
            AudioService.Instance.PlaySoundWithRandomPitch(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["HitObstacleSound"], EAudioType.SFX,
                hitVolumeRange, hitPitchRange);
            
            transform.position += new Vector3(-1, 0f, 0f);
            individualMultiplier -= obstacleHitDeduction.Value;

            int randomEmote = Random.Range(0, 2);

            switch (randomEmote)
            {
                case 0:
                        iconHandler.DisplayIcon(EEmotion.Sad);
                    break;
                default:
                        iconHandler.DisplayIcon(EEmotion.Embarrassed);
                    break;
            }

            OnHitObstacleEvents.Invoke();
        }

        public void OnClearedObstacle()
        {
            int randomEmote = Random.Range(0, 3);

            switch (randomEmote)
            {
                case 0:
                    iconHandler.DisplayIcon(EEmotion.Love);
                    break;
                default:
                    iconHandler.DisplayIcon(EEmotion.Happy);
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!canMove)
                return;

            rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * (Time.deltaTime * individualMultiplier), rb.linearVelocity.y,
                rb.linearVelocity.z);

            individualMultiplier += 0.001f;
        }
    }
}