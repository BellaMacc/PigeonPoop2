using UnityEngine;

public class LandingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GroundDetector groundDetection;
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private FlightMovement flightMovement;

    [Header("Landing Settings")]
    [SerializeField] private float landingDuration = 0.5f;
    [SerializeField] private float landingRotationSpeed = 360f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private Quaternion targetRotation;

    private float landingTimer;

    public bool IsLanding { get; private set; }

    // Check whether a landing can begin.
    public bool CanLand()
    {
        if (!stateManager.IsFlying())
            return false;

        if (flightMovement.VerticalVelocity >= 0f)
            return false;

        return groundDetection.TryFindLandingSurface(out RaycastHit hit);
    }

    // Detect and begin landing in one operation.
    public bool TryBeginLanding(CharacterController controller)
    {
        if (IsLanding || !stateManager.IsFlying())
            return false;

        if (flightMovement.VerticalVelocity >= 0f)
            return false;

        if (!groundDetection.TryFindLandingSurface(out RaycastHit hit))
            return false;

        BeginLanding(hit, controller);

        return true;
    }

    private void BeginLanding(
        RaycastHit hit,
        CharacterController controller)
    {
        IsLanding = true;
        landingTimer = 0f;

        startPosition = transform.position;

        // Calculate the player's rotation relative to the surface.
        Vector3 surfaceForward = Vector3.ProjectOnPlane(
            transform.forward,
            hit.normal
        ).normalized;

        if (surfaceForward.sqrMagnitude < 0.001f)
            surfaceForward = Vector3.ProjectOnPlane(
                transform.right,
                hit.normal
            ).normalized;

        targetRotation = Quaternion.LookRotation(
            surfaceForward,
            hit.normal
        );

        // Position the bottom of the CharacterController
        // directly against the detected surface.
        Vector3 controllerBottom =
            controller.center -
            Vector3.up * (controller.height * 0.5f);

        Vector3 worldBottomOffset =
            targetRotation * controllerBottom;

        targetPosition = hit.point - worldBottomOffset;

        stateManager.SetState(PlayerMovementState.Landing);
    }

    public Vector3 CalculateLandingMovement(float deltaTime)
    {
        if (!IsLanding)
            return Vector3.zero;

        landingTimer += deltaTime;

        float progress = Mathf.Clamp01(
            landingTimer / Mathf.Max(landingDuration, 0.01f)
        );

        // Smoothly ease into the landing position.
        float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

        Vector3 desiredPosition = Vector3.Lerp(
            startPosition,
            targetPosition,
            easedProgress
        );

        return (desiredPosition - transform.position) /
               Mathf.Max(deltaTime, 0.0001f);
    }

    public Quaternion CalculateLandingRotation(float deltaTime)
    {
        return Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            landingRotationSpeed * deltaTime
        );
    }

    public bool LandingComplete()
    {
        return IsLanding && landingTimer >= landingDuration;
    }

    public void FinishLanding()
    {
        IsLanding = false;
        landingTimer = 0f;
    }
}
