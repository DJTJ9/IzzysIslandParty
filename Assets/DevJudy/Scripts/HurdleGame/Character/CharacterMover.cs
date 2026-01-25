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
        // [SerializeField private AnimationCurve speedThroughLevelCurve; // curve that multiplies the speed based on how well the character is doing

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

        private void FixedUpdate()
        {
            if (canMove)
                rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * Time.deltaTime, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }
}