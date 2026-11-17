using UnityEngine;

public class Poop : MonoBehaviour
{
    [SerializeField] PooParticleManager pooParticles;
    [SerializeField] PoopMode pigeonModes;

    public void HandlePoop()
    {
        if (!pigeonModes.CanPoop())
        {
            pigeonModes.EnterPoopMode();
        }
        else
        {
            pooParticles.SpawnSphere();
            pigeonModes.ExitPoopMode();
        }
    }

}
