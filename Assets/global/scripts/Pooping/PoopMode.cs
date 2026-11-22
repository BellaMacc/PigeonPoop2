using PlayerLocomotion;
using UnityEditor.EditorTools;
using UnityEngine;

public class PoopMode : MonoBehaviour
{
    [SerializeField] PlayerStateManager stateManager;
    private bool isPoopMode = false;


    public bool IsPoopMode()
    {
        return isPoopMode;
    }

    public void HandleTogglePoopMode()
    {
        if(!isPoopMode && !stateManager.IsFlying()) { return; }

        if(!isPoopMode && stateManager.IsFlying())
        {
            stateManager.SetState(PlayerMovementState.PoopMode);
            isPoopMode=true;
            return;
        }

        if (isPoopMode) {

            stateManager.SetState(PlayerMovementState.Flying);
            isPoopMode=false;
            return;
        }
        
    }


}



