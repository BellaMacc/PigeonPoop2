using UnityEngine;


namespace PlayerLocomotion
{

    public class XZ_Movement : MonoBehaviour
    {
         [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Rigidbody player;
        [SerializeField] private Transform cameraObject;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 7f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationDampening = 0.1f;

        private Vector2 movementInput;

        private Vector3 moveDirection;
        private float targetAngle;
        private float turnSmoothVelocity;

        private const float INPUT_DEADZONE = 0.001f;

        private void Awake()
        {
            if (player == null)
                player = GetComponent<Rigidbody>();
        }

        public void OnMove()
        {
            movementInput = inputManager.ReadMovement();
        }

        private void CalculateMovement()
        {
            Vector3 inputDirection = new Vector3(
                movementInput.x,
                0f,
                movementInput.y
            );

            inputDirection = Vector3.ClampMagnitude(
                inputDirection, 1f
            );

            if (inputDirection.sqrMagnitude < INPUT_DEADZONE)
            {
                moveDirection = Vector3.zero;
                return;
            }

            // Camera-relative directions
            Vector3 forward = cameraObject.forward;
            Vector3 right = cameraObject.right;

            // Ignore camera pitch and roll
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection =
                forward * inputDirection.z +
                right * inputDirection.x;

            moveDirection = Vector3.ClampMagnitude(
                moveDirection, 1f
            );

            // Determine desired player rotation
            targetAngle = Mathf.Atan2(
                moveDirection.x,
                moveDirection.z
            ) * Mathf.Rad2Deg;
        }

        private void HandleMovement()
        {
            Vector3 velocity = player.linearVelocity;

            // Control horizontal movement only
            velocity.x = moveDirection.x * movementSpeed;
            velocity.z = moveDirection.z * movementSpeed;

            player.linearVelocity = velocity;
        }

        private void HandleRotation()
        {
            if (moveDirection.sqrMagnitude < INPUT_DEADZONE)
                return;

            float smoothAngle = Mathf.SmoothDampAngle(
                player.rotation.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                rotationDampening,
                Mathf.Infinity,
                Time.fixedDeltaTime
            );

            player.MoveRotation(
                Quaternion.Euler(0f, smoothAngle, 0f)
            );
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            HandleMovement();
            HandleRotation();
        }
    }

}

