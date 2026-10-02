using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject mySphere;
    public poopMeter poopMeter;
    public Transform spawnPoint;
    public void SpawnSphere()
    {
        if (poopMeter.poopNum > 0) {
            poopMeter.poopNum--;
            Instantiate(mySphere, spawnPoint.position, spawnPoint.rotation);
        }
        
    }
}
