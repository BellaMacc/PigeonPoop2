using UnityEngine;
// https://www.youtube.com/watch?v=wqydcq4kEEk&list=LL&index=1 I used this spawn vid to do my own spawning
public class SpawnTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject mySphere;
    public poopMeter poopMeter;
    public Transform spawnPoint;
    public AudioSource audioSource;
    public void SpawnSphere()
    {
        if (poopMeter.poopNum > 0) {
            poopMeter.poopNum--;
            audioSource.Play();
            Instantiate(mySphere, spawnPoint.position, spawnPoint.rotation);
        }
        
    }
}
