using Helper;
using UnityEngine;
using UnityEngine.Events;

namespace HurdleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        private Rigidbody rb;

        [SerializeField] private UnityEvent OnHitObstacleEvents;
        [SerializeField] private FloatReference moveSpeed;
        private float moveDirMultiplier = 100f;
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
            transform.position += new Vector3(-1, 0f, 0f);
            individualMultiplier -= 0.01f;

            // Play animation
            // Show Icon
            OnHitObstacleEvents.Invoke();
        }

        private void FixedUpdate()
        {
            // Debug.Log(gameObject.name + " CanMove: " + canMove);
            if (canMove)
                rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * (Time.deltaTime * individualMultiplier), rb.linearVelocity.y,
                    rb.linearVelocity.z);

            individualMultiplier += 0.001f;
        }
    }
}