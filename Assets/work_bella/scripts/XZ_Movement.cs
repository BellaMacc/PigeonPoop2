using UnityEngine;


namespace PlayerLocomotion
{

    public class XZ_Movement : MonoBehaviour
    {
        [SerializeField] InputManager inputManager;

        //public variables
        [SerializeField] float movementSpeed = 7;
        [SerializeField] float rotationSpeed = 5;
        [SerializeField] Transform cameraObject;
        [SerializeField] Rigidbody player;

        // movement variables
        Vector3 moveDirection;
     
        Vector2 movementInput;
        float verticalInput;
        float horizontalInput;

        Vector3 targetDirection = Vector3.zero;

        // direction variables

        private void Awake()
        {
            // inputManager = GetComponent<InputManager>();
           
        }
        public void OnMove()
        {
            movementInput = inputManager.ReadMovement();
            verticalInput = movementInput.y;
            horizontalInput = movementInput.x;
        }


        private void HandleMovement()
        {
            /*
            //forward value
            moveDirection = cameraObject.forward * verticalInput;
            moveDirection += cameraObject.right * horizontalInput;
            moveDirection.Normalize();

            //moveDirection.y = 0; // this is subject to change when introducing flying

            //adjusting speed based on float
            moveDirection *= movementSpeed;

            Vector3 movementVelocity = moveDirection;
            player.linearVelocity = moveDirection;*/
            //modify so that it only controls the xz plane of movement

            Vector3 forward = cameraObject.forward;
            Vector3 right = cameraObject.right;

            // Keep horizontal movement on the XZ plane
            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            moveDirection = forward * verticalInput;
            moveDirection += right * horizontalInput;

            // Prevent diagonal movement from being faster
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            moveDirection *= movementSpeed;

            // Only XZ movement
            player.linearVelocity = new Vector3( moveDirection.x, player.linearVelocity.y,moveDirection.z);
        }

        private void HandleRotation()
        {

            Vector3 targetDirection = cameraObject.forward * verticalInput;
            targetDirection += cameraObject.right * horizontalInput;

            // Keep rotation on the XZ plane
            targetDirection.y = 0f;

            // Don't calculate a new rotation if we're not moving
            if (targetDirection.sqrMagnitude < 0.001f)
                return;

            targetDirection.Normalize();

            Quaternion targetRotation =
                Quaternion.LookRotation(targetDirection, Vector3.up);

            Quaternion playerRotation =
                Quaternion.Slerp(
                    player.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

            player.transform.rotation = playerRotation;
        }

        private void FixedUpdate()
        {
            HandleRotation();
            HandleMovement();
           
        }


    }

}