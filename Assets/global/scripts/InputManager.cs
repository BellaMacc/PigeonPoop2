using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    // Movement
    [Header("Character Movement")]
    [SerializeField] private Vector2 movementInput;
    public UnityEvent OnMovement;

    // Camera
    [Header("Camera")]
    [SerializeField] private Vector2 cameraInput;
    public UnityEvent OnCameraMove;

    // Interactions
    [Header("Interaction Actions")]
    public UnityEvent OnFlap;
    public UnityEvent OnPoop;
    public UnityEvent OnBoost;
    public UnityEvent OnNoseDive;
    public UnityEvent OnNoseDiveReleased;
    public UnityEvent OnPoopModeToggle;

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Movement
        playerControls.GameControls.Move.performed +=i => HandleMovement(i.ReadValue<Vector2>());

        playerControls.GameControls.Move.canceled += i => HandleMovement(Vector2.zero);

        // Camera
        playerControls.GameControls.CameraMove.performed += i => HandleCamera(i.ReadValue<Vector2>());

        playerControls.GameControls.CameraMove.canceled += i => HandleCamera(Vector2.zero);

        // Interactions
        playerControls.GameControls.Flap.performed += i => HandleFlap();

        playerControls.GameControls.Poop.performed +=i => HandlePoop();

        playerControls.GameControls.Boost.performed += i => HandleBoost();

        playerControls.GameControls.NoseDive.performed += i => HandleNoseDive();
        playerControls.GameControls.NoseDive.canceled += i => HandleNoseDiveReleased();


        playerControls.GameControls.PoopModeToggle.performed += i => HandlePoopModeToggle();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();

        movementInput = Vector2.zero;
        cameraInput = Vector2.zero;
    }

    private void OnDestroy()
    {
        playerControls?.Dispose();
    }

    // Movement
    private void HandleMovement(Vector2 input)
    {
        movementInput = input;
        OnMovement.Invoke();
    }

    // Camera
    private void HandleCamera(Vector2 input)
    {
        cameraInput = input;
       // Debug.Log("Camera Input: " + input);
        OnCameraMove.Invoke();
    }

    // Interactions
    private void HandleFlap()
    {
        OnFlap.Invoke();
    }

    private void HandlePoop()
    {
        OnPoop.Invoke();
    }

    private void HandleBoost()
    {
        OnBoost.Invoke();
    }

    private void HandleNoseDive()
    {
        OnNoseDive.Invoke();
    }

    private void HandleNoseDiveReleased()
    {
        OnNoseDiveReleased.Invoke();
    }

    private void HandlePoopModeToggle()
    {
        OnPoopModeToggle.Invoke();
    }

    public Vector2 ReadMovement()
    {
        return movementInput;
    }

    public Vector2 ReadCameraMovement()
    {
        return cameraInput;
    }
}
