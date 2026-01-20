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
        private Vector3 colliderMoveDir;

        private float prevYVelocity;
        // [SerializeField private AnimationCurve speedThroughLevelCurve; // curve that multiplies the speed based on how well the character is doing
  
        private float spawnPositionX;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector3((moveDirMultiplier * moveSpeed.Value) * Time.deltaTime, rb.linearVelocity.y, 0f);
            
            prevYVelocity = rb.linearVelocity.y;
        }
    }
}