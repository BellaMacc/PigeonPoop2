using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private GroundMovement groundMovement;
    [SerializeField] private FlightMovement flightMovement;
    [SerializeField] private Transform movementReference;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedGravity = -2f;

    // Private Variables
    private CharacterController characterController;
    private Vector2 movementInput;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        groundMovement.enabled = false;
        flightMovement.enabled = false;
    }
    private void OnEnable()
    {
        stateManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        stateManager.OnStateChanged -= HandleStateChange;
    }
    private void HandleStateChange(PlayerMovementState state)
    {
        switch (state)
        {
            case PlayerMovementState.Walking:
                DisableAllMovement();
                groundMovement.enabled = true;
                break;
            case PlayerMovementState.Flying:
                DisableAllMovement();
                flightMovement.enabled = true;
                break;
        }

    }
    // Called by InputManager's OnMovement event
    public void HandleMovementInput()
    {
        movementInput = inputManager.ReadMovement();
    }

    public void HandleFlap()
    {
        if (!stateManager.IsFlying())
        {
            stateManager.SetState(PlayerMovementState.Flying);
        }
        flightMovement.HandleFlap();
    }

    private void DisableAllMovement()
    {
        flightMovement.enabled = false;
        groundMovement.enabled = false;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        Vector3 horizontalVelocity = Vector3.zero;

        // Select movement system based on current state
        switch (stateManager.CurrentState)
        {
            case PlayerMovementState.Walking:

                horizontalVelocity =
                    groundMovement.CalculateMovement(
                        movementInput,
                        movementReference,
                        transform,
                        deltaTime
                    );

                break;

            case PlayerMovementState.Flying:

                horizontalVelocity = flightMovement.CalculateMovement(
                    movementInput,
                    transform,
                    deltaTime
                );

                break;

            case PlayerMovementState.Landing:
                // LandingController will go here
                break;
        }

        // Apply gravity
        if (stateManager.CurrentState == PlayerMovementState.Walking)
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedGravity;
            }
            else
            {
                verticalVelocity += gravity * deltaTime;
            }
        }
        else if (stateManager.CurrentState == PlayerMovementState.Flying)
        {
            verticalVelocity = 0f;
        }

        // Combine horizontal and vertical velocity
        Vector3 finalVelocity = horizontalVelocity;

        // Walking uses PlayerMovement's gravity.
        // Flying already includes its vertical velocity.
        if (stateManager.CurrentState == PlayerMovementState.Walking)
        {
            finalVelocity.y = verticalVelocity;
        }

        characterController.Move(finalVelocity * deltaTime);
    }
}