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
        [SerializeField] private float fallAccelerationMultiplier = 1.5f;
        

        private float flightTimer;
        private float fallTime;
        private bool isFlying = false;

        private void StartFlight()
        {
            isFlying = true;
            flightTimer = suspensionTime;

            Vector3 velocity = player.linearVelocity;
            velocity.y = flightLaunchForce;
            player.linearVelocity = velocity;

            player.useGravity = false;
        }

        public void Flap()
        {
            if (!isFlying)
            {
                StartFlight();
                return;
            }

            Vector3 velocity = player.linearVelocity;
            velocity.y += flapBoost;
            player.linearVelocity = velocity;

            flightTimer = suspensionTime;
            fallTime = 0f;
        }

        private void FixedUpdate()
        {
            if (!isFlying)
                return;

            flightTimer -= Time.fixedDeltaTime;

            if (flightTimer <= 0f)
            {
                fallTime += Time.fixedDeltaTime;

                float fallAcceleration =
                    downwardAcceleration *
                    Mathf.Pow(fallAccelerationMultiplier, fallTime);

                Vector3 velocity = player.linearVelocity;
                velocity.y -= fallAcceleration * Time.fixedDeltaTime;
                player.linearVelocity = velocity;
            }
        }

        public bool IsFlying()
        {
            return isFlying;
        }
    }
}