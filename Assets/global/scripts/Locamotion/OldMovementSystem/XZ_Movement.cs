using UnityEngine;


namespace PlayerLocomotion
{

    public class XZ_Movement : MonoBehaviour
    {
        /*
        [Header("References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Rigidbody player;
        [SerializeField] private Transform cameraObject;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 7f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 240f;

        private Vector2 movementInput;
        private Vector3 moveDirection;
        private float targetAngle;

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

            inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

            if (inputDirection.sqrMagnitude < INPUT_DEADZONE)
            {
                moveDirection = Vector3.zero;
                return;
            }

            // Camera-relative directions
            Vector3 forward = cameraObject.forward;
            Vector3 right = cameraObject.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection =
                forward * inputDirection.z +
                right * inputDirection.x;

            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            targetAngle = Mathf.Atan2(
                moveDirection.x,
                moveDirection.z
            ) * Mathf.Rad2Deg;
        }

        private void HandleMovement()
        {
            Vector3 velocity = player.linearVelocity;

            velocity.x = moveDirection.x * movementSpeed;
            velocity.z = moveDirection.z * movementSpeed;

            player.linearVelocity = velocity;
        }

        private void HandleRotation()
        {
            if (moveDirection.sqrMagnitude < INPUT_DEADZONE)
                return;

            Quaternion targetRotation = Quaternion.Euler(
                0f,
                targetAngle,
                0f
            );

            Quaternion smoothRotation = Quaternion.RotateTowards(
                player.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            player.MoveRotation(smoothRotation);
        }

        private void FixedUpdate()
        {
            CalculateMovement();
            HandleMovement();
            HandleRotation();
        }*/

        private void OnEnable()
        {
            Debug.Log("You are using an outdated movement file, make sure to have the latest character controller updated - bm");
        }
    }

}

