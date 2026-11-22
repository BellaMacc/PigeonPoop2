using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private GroundMovement groundMovement;
    [SerializeField] private FlightMovement flightMovement;
    [SerializeField] private LandingController landingController;
    [SerializeField] private Transform movementReference;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedGravity = -2f;

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

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;

        CheckForLanding();

        Vector3 movement = CalculateMovement(deltaTime);

        ApplyMovement(movement, deltaTime);

        CheckLandingCompletion();
    }
    //---------------------------------------------------------------------------------------------HELPER FUNCTIONS BELOW
    //--------------------------------------------------------------------------------------STATES

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

            case PlayerMovementState.Landing:
                DisableAllMovement();
                break;
        }
    }

    private void DisableAllMovement()
    {
        flightMovement.enabled = false;
        groundMovement.enabled = false;
    }

    //--------------------------------------------------------------------------------------INPUTS
    public void HandleMovementInput()
    {
        movementInput = inputManager.ReadMovement();
    }

    public void HandleFlap()
    {
        if (stateManager.IsLanding())
            return;

        if (!stateManager.IsFlying())
            stateManager.SetState(PlayerMovementState.Flying);

        flightMovement.HandleFlap();
    }

   //--------------------------------------------------------------------------------------LANDING
    private void CheckForLanding()
    {
        if (!stateManager.IsFlying())
            return;

        landingController.TryBeginLanding(characterController);
    }

    private void CheckLandingCompletion()
    {
        if (!stateManager.IsLanding())
            return;

        if (landingController.LandingComplete())
            FinishLanding();
    }

    private void FinishLanding()
    {
        landingController.FinishLanding();

        flightMovement.ResetFlight();
        groundMovement.ResetMovement();

        verticalVelocity = groundedGravity;
        movementInput = Vector2.zero;

        stateManager.SetState(PlayerMovementState.Walking);
    }
    //--------------------------------------------------------------------------------------MOVEMENT CALCULATIONS

    private Vector3 CalculateMovement(float deltaTime)
    {
        switch (stateManager.CurrentState)
        {
            case PlayerMovementState.Walking:
                return CalculateWalkingMovement(deltaTime);

            case PlayerMovementState.Flying:
                return CalculateFlyingMovement(deltaTime);

            case PlayerMovementState.Landing:
                return CalculateLandingMovement(deltaTime);

            default:
                return Vector3.zero;
        }
    }

    private Vector3 CalculateWalkingMovement(float deltaTime)
    {
        return groundMovement.CalculateMovement(
            movementInput,
            movementReference,
            transform,
            deltaTime
        );
    }

    private Vector3 CalculateFlyingMovement(float deltaTime)
    {
        return flightMovement.CalculateMovement(
            movementInput,
            transform,
            deltaTime
        );
    }

    private Vector3 CalculateLandingMovement(float deltaTime)
    {
        transform.rotation = landingController.CalculateLandingRotation(
            deltaTime
        );

        return landingController.CalculateLandingMovement(deltaTime);
    }


    //--------------------------------------------------------------------------------------APPLY CALCULATIONS
    private void ApplyMovement(Vector3 movement, float deltaTime)
    {
        ApplyGravity(deltaTime);

        if (stateManager.IsWalking())
            movement.y = verticalVelocity;

        characterController.Move(movement * deltaTime);
    }

    private void ApplyGravity(float deltaTime)
    {
        if (stateManager.IsWalking())
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
        else if (stateManager.IsFlying())
        {
            verticalVelocity = 0f;
        }
    }
}