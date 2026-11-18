using PlayerLocomotion;
using UnityEditor.EditorTools;
using UnityEngine;

public class PoopMode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Flight flight;
    [SerializeField] private Poop poopManager;

    [Header("Poop Mode Settings")]
    [SerializeField] private float poopModeDuration = 10f;

    private float poopTimer;
    private bool isPoopMode;

    public bool IsPoopMode => isPoopMode;

    // Called through InputManager.OnPoop
    public void EnterPoopMode()
    {
        if (isPoopMode)
            return;

        isPoopMode = true;
        poopTimer = poopModeDuration;

        cameraManager.SetPoopCamera(true);
        flight.SetPoopGlide(true);

        Debug.Log("Entered Poop Mode");
    }

    private void Update()
    {
        if (!isPoopMode)
            return;

        poopTimer -= Time.deltaTime;

        if (poopTimer <= 0f)
        {
            ExitPoopMode();
        }
    }

    public void ExitPoopMode()
    {
        if (!isPoopMode)
            return;

        isPoopMode = false;

        cameraManager.SetPoopCamera(false);
        flight.SetPoopGlide(false);

        Debug.Log("Exited Poop Mode");
    }

    public bool CanPoop()
    {
        return isPoopMode;
    }
}



