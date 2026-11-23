using UnityEngine;

public class FlightMovement : MonoBehaviour
{

    [Header("Flight References")]
    [SerializeField] private Transform flightDirection;

    [Header("Forward Movement")]
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float speedAdjustment = 4f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float  currentSpeed = 0f;

    [Header("Steering")]
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float turnEaseInTime = 0.06f;
    [SerializeField] private float turnEaseOutTime = 0.25f;

    [Header("Flapping")]
    [SerializeField] private float flapForce = 14f;

    [Header("Falling")]
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float maxFallSpeed = 7f;

    [Header("Nose Dive")]
    [SerializeField] private float noseDiveGravityMultiplier = 2.5f;
    [SerializeField] private float noseDiveMaxFallSpeed = 18f;
    [SerializeField] private float noseDiveImpulse = 5f;

   

    [Header("Obstacle Detection")]
    [SerializeField] private FlightObstacteDetection obstacleDetector;
    [SerializeField] private float obstacleSlowdownDistance = 3f;
    [SerializeField] private float minimumObstacleSpeed = 1.5f;
    [SerializeField] private float obstacleDeceleration = 12f;
    [SerializeField] private float obstacleRecovery = 6f;
    

    private bool isNoseDiving;
    public float VerticalVelocity => verticalVelocity;

   
    private float verticalVelocity;

    private float currentTurnSpeed;
    private float turnSmoothVelocity;

    private void Awake()
    {
        currentSpeed = baseSpeed;
    }

    public void HandleFlap()
    {
        verticalVelocity += flapForce;
    }


    public Vector3 CalculateMovement(
        Vector2 input,
        Transform player,
        float deltaTime)
    {
        CalculateSteering(input, player, deltaTime);

        // Speed calculation

        float targetSpeed = CalculateTargetSpeed(input);

        RaycastHit hit = default;
        bool obstacleDetected = obstacleDetector != null && obstacleDetector.TryDetectObstacle(player, out hit);

        if (obstacleDetected)
        {
            float distanceFactor = Mathf.Clamp01(hit.distance / obstacleSlowdownDistance);
            targetSpeed = Mathf.Lerp(minimumObstacleSpeed, targetSpeed, distanceFactor);
        }

        float speedChangeRate = obstacleDetected
            ? (targetSpeed < currentSpeed ? obstacleDeceleration : obstacleRecovery)
            : (targetSpeed > currentSpeed ? acceleration : deceleration);

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedChangeRate * deltaTime);

        // Vertical movement
       
        float activeGravity = isNoseDiving ? gravity * noseDiveGravityMultiplier : gravity;
        float activeMaxFallSpeed = isNoseDiving ? noseDiveMaxFallSpeed : maxFallSpeed;

        verticalVelocity += activeGravity * deltaTime;
        verticalVelocity = Mathf.Max(verticalVelocity, -activeMaxFallSpeed);

        // Final movement
        Vector3 forwardVelocity = flightDirection.forward * currentSpeed;

        forwardVelocity.y += verticalVelocity;

        return forwardVelocity;
    }
    // STEERING
    private void CalculateSteering(
        Vector2 input,
        Transform player,
        float deltaTime)
    {
        float targetTurnSpeed = input.x * turnSpeed;

        float smoothTime = Mathf.Abs(input.x) > 0.01f ? turnEaseInTime: turnEaseOutTime;

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
    }

    private float CalculateTargetSpeed(Vector2 input)
    {
        if (input.y > 0.1f)
            return maxSpeed;

        if (input.y < -0.1f)
            return minSpeed;

        return baseSpeed;
    }


    // OBSTACLE DETECTIOn
    private float AdjustForObstacles(float targetSpeed, Transform player)
    {
        RaycastHit hit = default;
        bool obstacleDetected = obstacleDetector != null && obstacleDetector.TryDetectObstacle(player, out hit);

        if (obstacleDetected)
        {
            float distanceFactor = Mathf.Clamp01(hit.distance / obstacleSlowdownDistance);
            targetSpeed = Mathf.Lerp(minimumObstacleSpeed, targetSpeed, distanceFactor);
        }

        return targetSpeed;
    }

    public void SetNoseDive(bool active)
    {
        if (active && !isNoseDiving)
            verticalVelocity -= noseDiveImpulse;

        if (!active && isNoseDiving)
            verticalVelocity = Mathf.Max(verticalVelocity, -maxFallSpeed);

        isNoseDiving = active;
    }

    public void ResetFlight()
    {
        currentSpeed = baseSpeed;
        verticalVelocity = 0f;

        currentTurnSpeed = 0f;
        turnSmoothVelocity = 0f;
        isNoseDiving = false;
    }

    public float GetCurrentSpeed() { return currentSpeed; }
    public float GetBaseSpeed() { return baseSpeed; }
    public float GetMaxSpeed() { return maxSpeed; }
}


