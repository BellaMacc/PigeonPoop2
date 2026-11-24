using UnityEngine;
using System;

public class PlayerStateManager : MonoBehaviour
{
    public PlayerMovementState CurrentState { get; private set; } = PlayerMovementState.Walking;

    public event Action<PlayerMovementState> OnStateChanged;

   


   
    public void SetState(PlayerMovementState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        OnStateChanged?.Invoke(CurrentState);
    }
    //-----------------------------------------------------Grounded
    public bool IsWalking()
    {
        return CurrentState == PlayerMovementState.Walking;
    }
    //-----------------------------------------------------Flying
    public bool IsFlying()
    {
        return CurrentState == PlayerMovementState.Flying;
    }

    public bool IsLanding()
    {
        return CurrentState == PlayerMovementState.Landing;
    }
    public bool IsPoopMode()
    {
        return CurrentState == PlayerMovementState.PoopMode;
    }

    //-----------------------------------------------------Pooping

}


