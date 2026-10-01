using UnityEngine;

namespace PlayerLocomotion
{
    public class Flight : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Rigidbody player;

        [Header("Flight Settings")]
        [SerializeField] private float suspensionTime = 5f;
        [SerializeField] private float flapBoost = 5f;
        [SerializeField] private float downwardAcceleration = 1f;
        [SerializeField] private float flightLaunchForce = 8f;

        private float flightTimer;
        private bool isFlying = false;

        private void OnEnable()
        {
            
            //inputManager.OnFlap.AddListener(Flap);
        }
           
        private void OnDisable()
        {
           
            //inputManager.OnFlap.RemoveListener(Flap);
        }

        private void StartFlight()
        {
            isFlying = true;
            flightTimer = suspensionTime;

            // Stop existing vertical movement
            Vector3 velocity = player.linearVelocity;
            velocity.y = flightLaunchForce;
            player.linearVelocity = velocity;

            // Handle vertical movement ourselves
            player.useGravity = false;
        }

        public void Flap()
        {
            if (!isFlying)
            {
                StartFlight();
            }

            // upward burst
            SetVerticalVelocity(flapBoost);

            //another period of suspension
            flightTimer = suspensionTime;
        }

        private void FixedUpdate()
        {
            if (!isFlying)
                return;

            flightTimer -= Time.fixedDeltaTime;

            if (flightTimer > 0f)
            {
                if (player.linearVelocity.y < 0f)
                {
                    SetVerticalVelocity(0f);
                }
            }
            else
            {
                // Begin a slow descent
                ApplySlowFall();
            }
        }

        private void ApplySlowFall()
        {
            Vector3 velocity = player.linearVelocity;

            velocity.y -= downwardAcceleration * Time.fixedDeltaTime;

            player.linearVelocity = velocity;
        }

        private void SetVerticalVelocity(float verticalVelocity)
        {
            Vector3 velocity = player.linearVelocity;

            velocity.y = verticalVelocity;

            player.linearVelocity = velocity;
        }
    }
}