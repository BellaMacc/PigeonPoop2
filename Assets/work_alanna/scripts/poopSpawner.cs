using UnityEngine;

public class poopSpawner : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    public poopMeter meter;
    public GameObject poop;

    public void Poop()
    {
        if (meter.poopNum != 0)
        {
            Instantiate(poop);
            meter.poopNum--;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
