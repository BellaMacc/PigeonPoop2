using UnityEngine;

public class FlightMovement : MonoBehaviour
{

    [Header("Flight References")]
    [SerializeField] private Transform flightDirection;

    [Header("Forward Movement")]
    [SerializeField] private float baseSpeed = 8f;
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float minSpeed = 4f;
    [SerializeField] private float speedAdjustment = 4f;

    [Header("Steering")]
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float turnEaseInTime = 0.06f;
    [SerializeField] private float turnEaseOutTime = 0.25f;

    [Header("Flapping")]
    [SerializeField] private float flapForce = 12f;

    [Header("Falling")]
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float maxFallSpeed = 18f;

    private float currentSpeed;
    private float verticalVelocity;

    private float currentTurnSpeed;
    private float turnSmoothVelocity;

    private void Awake()
    {
        currentSpeed = baseSpeed;
    }

    // Called whenever the player presses flap
    public void HandleFlap()
    {
        // Every flap adds an immediate upward boost.
        // This works while ascending, suspended or falling.
        verticalVelocity += flapForce;
    }

    public Vector3 CalculateMovement(
        Vector2 input,
        Transform player,
        float deltaTime)
    {
        // --------------------------------
        // STEERING
        // --------------------------------

        float targetTurnSpeed = input.x * turnSpeed;

        // Respond quickly when steering.
        // Ease out more gradually when the stick is released.
        float smoothTime = Mathf.Abs(input.x) > 0.01f
            ? turnEaseInTime
            : turnEaseOutTime;

        currentTurnSpeed = Mathf.SmoothDamp(
            currentTurnSpeed,
            targetTurnSpeed,
            ref turnSmoothVelocity,
            smoothTime,
            Mathf.Infinity,
            deltaTime
        );

        player.Rotate(
            Vector3.up,
            currentTurnSpeed * deltaTime,
            Space.World
        );

        // --------------------------------
        // FORWARD SPEED
        // --------------------------------

        float targetSpeed = baseSpeed + input.y * speedAdjustment;

        targetSpeed = Mathf.Clamp(
            targetSpeed,
            minSpeed,
            maxSpeed
        );

        currentSpeed = targetSpeed;

        // --------------------------------
        // VERTICAL MOVEMENT
        // --------------------------------

        // Gravity acts continuously.
        // Flaps provide immediate upward impulses.
        verticalVelocity += gravity * deltaTime;

        // Limit downward speed without limiting upward flap boosts.
        verticalVelocity = Mathf.Max(
            verticalVelocity,
            -maxFallSpeed
        );

        // --------------------------------
        // FINAL MOVEMENT
        // --------------------------------

        Vector3 forwardVelocity = flightDirection.forward * currentSpeed;

        forwardVelocity.y += verticalVelocity;

        return forwardVelocity;
    }

    public void ResetFlight()
    {
        currentSpeed = baseSpeed;

        verticalVelocity = 0f;

        currentTurnSpeed = 0f;
        turnSmoothVelocity = 0f;
    }
}

