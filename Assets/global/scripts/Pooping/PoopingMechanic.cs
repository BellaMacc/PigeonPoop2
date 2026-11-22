using UnityEngine;

public class PoopingMechanic : MonoBehaviour
{
    [SerializeField] PoopMode poopMode;
    [SerializeField] PoopRaycast poopRaycast;
    [SerializeField] PigeonData pigeonData;
    [SerializeField] PooParticleManager poopVisuals;

    public void HandlePoopPress()
    {
        if (!poopMode.IsPoopMode()) { return; }
        if (!pigeonData.CanPoop()) { return; }

        poopVisuals.StartPooParticles();
        bool gotPoint = poopRaycast.CastDownward();
        if (gotPoint) { Debug.Log("hit someone!"); }

    }

}
