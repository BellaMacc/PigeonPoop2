using UnityEngine;

public class Poop : MonoBehaviour
{
    [SerializeField] PooParticleManager pooParticles;
    [SerializeField] PoopMode pigeonModes;
    [SerializeField] PigeonManager pigeonManager;

    public void HandlePoop()
    {
        if (!pigeonModes.CanPoop() && pigeonManager.CanPoop())
        {
            pigeonModes.EnterPoopMode();
        }
        else if (pigeonModes.CanPoop() && pigeonManager.CanPoop()) 
        {
            pigeonManager.HandlePoop();
            pooParticles.SpawnSphere();
            pigeonModes.ExitPoopMode();
        }
    }

}
