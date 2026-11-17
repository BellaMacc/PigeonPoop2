using UnityEngine;

namespace PlayerLocomotion
{
    public class Flight : MonoBehaviour
    {
       
        [Header("References")]
        [SerializeField] private Rigidbody player;
        [SerializeField] private Transform groundCheck;

        [Header("Flight Settings")]
        [SerializeField] private float flapForce = 7f;
        [SerializeField] private float maxUpwardVelocity = 12f;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private float groundCheckDistance = 0.25f;

        private bool isGrounded;

        public bool IsGrounded => isGrounded;
        public bool IsAirborne => !isGrounded;

        private void Awake()
        {
            if (player == null)
                player = GetComponent<Rigidbody>();
        }

        public void Flap()
        {
            // Reset downward momentum or boost existing upward momentum
            Vector3 velocity = player.linearVelocity;

            velocity.y = Mathf.Min(
                Mathf.Max(velocity.y, 0f) + flapForce,
                maxUpwardVelocity
            );

            player.linearVelocity = velocity;

            // Prevent immediate ground detection after takeoff
            isGrounded = false;
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.SphereCast(
                groundCheck.position,
                groundCheckRadius,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundLayers,
                QueryTriggerInteraction.Ignore
            );

            if (isGrounded && player.linearVelocity.y < 0f)
            {
                Vector3 velocity = player.linearVelocity;
                velocity.y = 0f;
                player.linearVelocity = velocity;
            }
        }

        private void FixedUpdate()
        {
            CheckGrounded();
        }
        
    }
}
