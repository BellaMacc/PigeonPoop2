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

    private void Awake()
    {
        playerControls = new PlayerControls();

        // Movement
        playerControls.Movement.Move.performed +=
            i => HandleMovement(i.ReadValue<Vector2>());

        playerControls.Movement.Move.canceled +=
            i => HandleMovement(Vector2.zero);

        // Camera
        playerControls.Camera.CameraMove.performed +=
            i => HandleCamera(i.ReadValue<Vector2>());

        playerControls.Camera.CameraMove.canceled +=
            i => HandleCamera(Vector2.zero);

        // Interactions
        playerControls.Interactions.Flap.performed +=
            i => HandleFlap();

        playerControls.Interactions.Poop.performed +=
            i => HandlePoop();

        playerControls.Interactions.Boost.performed +=
            i => HandleBoost();

        playerControls.Interactions.NoseDive.performed +=
            i => HandleNoseDive();
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
        //Debug.Log("Camera Input: " + input);
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

    public Vector2 ReadMovement()
    {
        return movementInput;
    }

    public Vector2 ReadCameraMovement()
    {
        return cameraInput;
    }
}
