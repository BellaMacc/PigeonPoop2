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
            Vector3 targetDirection = Vector3.zero;

            targetDirection = cameraObject.forward * verticalInput;
            targetDirection += cameraObject.right * horizontalInput;

            targetDirection.y = 0;
            targetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            player.transform.rotation = playerRotation;
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleRotation();
        }


    }

}