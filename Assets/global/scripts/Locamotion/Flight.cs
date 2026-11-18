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
        [SerializeField] private float suspensionDuration = 0.5f;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private float groundCheckDistance = 0.25f;

        [SerializeField] private bool isGrounded;
        private bool isPoopGliding;

        private float suspensionTimer;

        private bool defaultGravityState;

        public bool IsGrounded => isGrounded;
        public bool IsAirborne => !isGrounded;
        public bool IsPoopGliding => isPoopGliding;

        private void Awake()
        {
            if (player == null)
                player = GetComponent<Rigidbody>();

            defaultGravityState = player.useGravity;
        }

        public void Flap()
        {
            if (isPoopGliding)
                return;

            Vector3 velocity = player.linearVelocity;

            velocity.y = Mathf.Min(
                Mathf.Max(velocity.y, 0f) + flapForce,
                maxUpwardVelocity
            );

            player.linearVelocity = velocity;

            isGrounded = false;
            suspensionTimer = suspensionDuration;

            UpdateGravity();
        }

        public void SetPoopGlide(bool active)
        {
            if (isPoopGliding == active)
                return;

            isPoopGliding = active;

            if (active)
            {
                // Stop vertical movement during poop mode
                Vector3 velocity = player.linearVelocity;
                velocity.y = 0f;
                player.linearVelocity = velocity;
            }
            else
            {
                // Give the bird some grace time before falling
                suspensionTimer = suspensionDuration;
            }

            UpdateGravity();
        }

        private void UpdateGravity()
        {
            player.useGravity =
                defaultGravityState &&
                !isPoopGliding &&
                suspensionTimer <= 0f;
        }

        private void CheckGrounded()
        {
            // Don't consider the bird grounded while moving upward.
            if (player.linearVelocity.y > 0.01f)
            {
                isGrounded = false;
                return;
            }

            isGrounded = Physics.SphereCast(
                groundCheck.position,
                groundCheckRadius,
                Vector3.down,
                out _,
                groundCheckDistance,
                groundLayers,
                QueryTriggerInteraction.Ignore
            );

            if (isGrounded)
            {
                suspensionTimer = 0f;

                Vector3 velocity = player.linearVelocity;

                if (velocity.y < 0f)
                    velocity.y = 0f;

                player.linearVelocity = velocity;

                UpdateGravity();
            }
        }

        private void FixedUpdate()
        {
            if (isPoopGliding)
            {
                Vector3 velocity = player.linearVelocity;
                velocity.y = 0f;
                player.linearVelocity = velocity;

                return;
            }

            if (suspensionTimer > 0f)
            {
                suspensionTimer -= Time.fixedDeltaTime;

                if (suspensionTimer <= 0f)
                {
                    suspensionTimer = 0f;
                    UpdateGravity();
                }
            }

            CheckGrounded();
        }
    }
}

