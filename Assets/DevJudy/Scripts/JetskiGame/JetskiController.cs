using Ditzelgames;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JetskiGame
{
    public class JetskiController : MonoBehaviour
    {
        [SerializeField] private Transform motor;
        private Rigidbody rb;

        [SerializeField] private float power = 5f;
        [SerializeField] private float steerPower = 500f;
        [SerializeField] private float maxSpeed = 10f;

        private Vector2 moveInput = Vector2.zero;
        private bool driving = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        public void OnMove(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                moveInput = _context.ReadValue<Vector2>();

                if (moveInput.y != 0)
                    driving = true;
            }

            if (_context.canceled)
            {
                moveInput = Vector2.zero;
                driving = false;
            }
        }

        public void OnJump(InputAction.CallbackContext _context)
        {
            if (_context.started)
            {
                // Let the jetski jump slightly in the air
                Debug.Log("Jump");
            }
        }

        private void FixedUpdate()
        {
            rb.AddForceAtPosition(transform.right * (-moveInput.x * steerPower) / 100f, motor.position);

            var forward = Vector3.Scale(new Vector3(1, 0, 1), transform.forward);

            PhysicsHelper.ApplyForceToReachVelocity(rb, forward * (maxSpeed * moveInput.y), power);

            // if (driving)
            //     If particleSystem
            // not playing, play

            // if (!driving)
            //     If particleSystem
            // not playing, play
        }
    }
}