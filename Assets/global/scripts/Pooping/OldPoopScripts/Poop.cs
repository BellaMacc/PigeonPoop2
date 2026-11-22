using UnityEngine;

public class Poop : MonoBehaviour
{
    [SerializeField] PooParticleManager pooParticles;
    [SerializeField] PoopMode pigeonModes;
    [SerializeField] PigeonManager pigeonManager;
    [SerializeField] PoopRaycast pooCast;

    public void HandlePoop()
    {
        /*
        if (!pigeonModes.CanPoop() && pigeonManager.CanPoop())
        {
            pigeonModes.EnterPoopMode();
        }
        else if (pigeonModes.CanPoop() && pigeonManager.CanPoop()) 
        {
            pigeonManager.HandlePoop();
            pooParticles.SpawnSphere();
            bool gotPoint = pooCast.CastDownward();
            if (gotPoint)
            {
                pigeonManager.GivePoints();
            }
            pigeonModes.ExitPoopMode();
        }*/
    }

    private void OnEnable()
    {
        Debug.Log("Depricate File 'poop.cs' male sure to update to new character controller - BM");
    }

}

