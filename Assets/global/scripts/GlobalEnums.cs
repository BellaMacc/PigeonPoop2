using UnityEngine;


//Used in Collider Boundary, Collision Detection
public enum BoundaryType
{
    Default,
    River,
    Train,
    Construction,
    Sky,
    Park,
    City,
}
//Used to determine playermovement state
//and switch between them in PlayerMovement and in PlayerState
public enum PlayerMovementState
{
    Walking,
    Flying,
    PoopMode,
    Landing
}

//Used to determine camera mode in
//player state and in camera manager
public enum CameraMode
{
    Walking,
    Flight,
    Poop
}
