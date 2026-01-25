using UnityEngine;

namespace HurdleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        private Rigidbody rb;

        [Header("Movement variables: ")]
        [SerializeField] private FloatReference moveSpeed;

        private float moveDirMultiplier = 100f;

        private float individualMultiplier = 1f;
        public float IndividualMultiplier
        {
            get => individualMultiplier;
            set => individualMultiplier = value;
        }
        
        [SerializeField] private bool canMove = false;

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

        public void HitObstacle()
        {
            // Play animation
            // Show Icon
            
            transform.position += new Vector3(-1, 0f, 0f);
            individualMultiplier -= 0.01f;
        }

        private void FixedUpdate()
        {
            if (canMove)
                rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * (Time.deltaTime * individualMultiplier), rb.linearVelocity.y,
                    rb.linearVelocity.z);

            individualMultiplier += 0.001f;
        }
    }
}