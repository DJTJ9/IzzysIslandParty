using UnityEngine;

namespace HurdleGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterMover : MonoBehaviour
    {
        private Rigidbody rb;

        [Header("Movement variables: ")]
        [SerializeField] private FloatReference moveSpeed;
        [SerializeField] private FloatReference velocity;
        private float moveDirMultiplier = 100f;
        private Vector3 colliderMoveDir;

        private float prevYVelocity;
        // [SerializeField private AnimationCurve speedThroughLevelCurve; // curve that multiplies the speed based on how well the character is doing
  
        private float spawnPositionX;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
        }


        private void FixedUpdate()
        {
            // !! This shit keeps bopping up and down
            rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * Time.deltaTime, rb.linearVelocity.y, rb.linearVelocity.z);
            
            
            //Ditzelgames.PhysicsHelper.ApplyForceToReachVelocity(rb, new Vector3(velocity.Value, 0f, 0f), (moveSpeed.Value * moveDirMultiplier) * Time.deltaTime);
            
            prevYVelocity = rb.linearVelocity.y;
        }
    }
}