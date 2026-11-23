using UnityEngine;

public class FlightRotationAnim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform birdModel;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerStateManager stateManager;
    [SerializeField] private FlightMovement flightMovement;

    [Header("Banking")]
    [SerializeField] private float bankBuildSpeed = 45f;
    [SerializeField] private float maxBankAngle = 25f;
    [SerializeField] private float bankRecoverySpeed = 35f;
    [SerializeField] private float bankSmoothness = 5f;

    [Header("Flap Lift")]
    [SerializeField] private float flapLiftAmount = 0.15f;
    [SerializeField] private float flapLiftDuration = 0.3f;
    [SerializeField] private float flapLiftSmoothness = 5f;

    


    [Header("Descending")]
    [SerializeField] private float descentPitch = 8f;
    [SerializeField] private float descentDropAmount = 0.1f;
    [SerializeField] private float descentSensitivity = 5f;
    [SerializeField] private float descentSmoothness = 5f;

    private Vector2 movementInput;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private float bankTarget;
    private float currentBank;
    private float flapTimer;
    private float currentDescent;
    private float currentDrop;
    private float currentFlapLift;

    private void Awake()
    {
        defaultPosition = birdModel.localPosition;
        defaultRotation = birdModel.localRotation;
    }
    /*
    private void OnEnable()
    {
        inputManager.OnMovement.AddListener(HandleMovementInput);
        inputManager.OnFlap.AddListener(HandleFlap);
    }

    private void OnDisable()
    {
        inputManager.OnMovement.RemoveListener(HandleMovementInput);
        inputManager.OnFlap.RemoveListener(HandleFlap);
    }
    */
    public void HandleMovementInput()
    {
        movementInput = inputManager.ReadMovement();
    }

    public void HandleFlap()
    {
        if (!stateManager.IsFlying()) return;
        flapTimer = flapLiftDuration;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!stateManager.IsFlying())
        {
            ResetVisuals(deltaTime);
            return;
        }

        UpdateBanking(deltaTime);
        UpdateFlapLift(deltaTime);
        UpdateDescent(deltaTime);
        ApplyVisuals();
    }

    private void UpdateBanking(float deltaTime)
    {
        if (Mathf.Abs(movementInput.x) > 0.1f)
        {
            bankTarget += movementInput.x * bankBuildSpeed * deltaTime;
            bankTarget = Mathf.Clamp(bankTarget, -maxBankAngle, maxBankAngle);
        }
        else
        {
            bankTarget = Mathf.MoveTowards(bankTarget, 0f, bankRecoverySpeed * deltaTime);
        }

        currentBank = Mathf.Lerp(currentBank, bankTarget, bankSmoothness * deltaTime);
    }

    private void UpdateFlapLift(float deltaTime)
    {
        flapTimer = Mathf.Max(0f, flapTimer - deltaTime);

        float progress = flapLiftDuration > 0f ? 1f - (flapTimer / flapLiftDuration) : 1f;
        float targetLift = flapLiftAmount * (1f - Mathf.SmoothStep(0f, 1f, progress));

        currentFlapLift = Mathf.Lerp(currentFlapLift, targetLift, flapLiftSmoothness * deltaTime);
    }

    private void UpdateDescent(float deltaTime)
    {
        float downwardSpeed = Mathf.Max(0f, -flightMovement.VerticalVelocity);
        float descentFactor = Mathf.Clamp01(downwardSpeed / descentSensitivity);

        currentDescent = Mathf.Lerp(currentDescent, descentFactor, descentSmoothness * deltaTime);
        currentDrop = Mathf.Lerp(currentDrop, descentFactor * descentDropAmount, descentSmoothness * deltaTime);
    }

    private void ApplyVisuals()
    {
        float flapFactor = flapLiftDuration > 0f ? flapTimer / flapLiftDuration : 0f;

        Quaternion bankRotation = Quaternion.Euler(descentPitch * currentDescent, 0f, -currentBank);

        birdModel.localRotation = defaultRotation * bankRotation;
        birdModel.localPosition = defaultPosition + Vector3.up * (currentFlapLift - currentDrop);
    }

    private void ResetVisuals(float deltaTime)
    {
        movementInput = Vector2.zero;
        bankTarget = 0f;
        flapTimer = 0f;

        currentBank = Mathf.Lerp(currentBank, 0f, bankSmoothness * deltaTime);
        currentDescent = Mathf.Lerp(currentDescent, 0f, descentSmoothness * deltaTime);
        currentDrop = Mathf.Lerp(currentDrop, 0f, descentSmoothness * deltaTime);
        currentFlapLift = Mathf.Lerp(currentFlapLift, 0f, flapLiftSmoothness * deltaTime);

        birdModel.localRotation = defaultRotation * Quaternion.Euler(descentPitch * currentDescent, 0f, -currentBank);
        birdModel.localPosition = defaultPosition + Vector3.down * currentDrop;
    }
}
