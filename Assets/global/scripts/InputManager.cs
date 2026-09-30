using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class InputManager: MonoBehaviour
{
    PlayerControls playerControls;


    //Movement Action
    [Header("Character Movement")]
    [SerializeField] Vector2 movementInput;
    public UnityEvent OnMovement;

    //Camera Action
    [Header("Camera")]
    [SerializeField] Vector2 cameraInput;
    public UnityEvent OnCameraMove;

    //Interaction Action
    [Header("Interactions Actions")]
    public UnityEvent OnFlap;
    public UnityEvent OnPoop;
    public UnityEvent OnBoost;
    public UnityEvent OnNoseDive;


    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();


            //This is the section that controls what happens when each button is pressed----------------------------------

            //Flying Action
            playerControls.Movement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.Movement.Move.performed += i => OnMovement.Invoke();
            //Camera Action
            playerControls.Camera.CameraMove.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.Camera.CameraMove.performed += i => OnCameraMove.Invoke();

            //Interactions

            playerControls.Interactions.Flap.performed += i => HandleFlap();
            playerControls.Interactions.Poop.performed += i => HandlePoop();
            playerControls.Interactions.Boost.performed += i => HandleBoost();
            playerControls.Interactions.NoseDive.performed += i => HandleNoseDive();
        }

        playerControls.Enable();
    }


    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void HandleFlap()
    {
        Debug.Log("Flap performed");
        OnFlap.Invoke();
    }

    private void HandlePoop()
    {
        Debug.Log("Poop performed");
        OnPoop.Invoke();
    }
    private void HandleBoost() {
        Debug.Log("Boost performed");
        OnBoost.Invoke();
    }

    private void HandleNoseDive()
    {
        Debug.Log("Nose Dive performed");
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
