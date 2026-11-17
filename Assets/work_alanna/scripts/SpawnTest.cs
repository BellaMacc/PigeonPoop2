using UnityEngine;
using System.Collections;

// https://www.youtube.com/watch?v=wqydcq4kEEk&list=LL&index=1 I used this spawn vid to do my own spawning
public class SpawnTest : MonoBehaviour
{
    public ParticleSystem poopParticles;
    public float shootDuration = 0.1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public GameObject mySphere;
    public poopMeter poopMeter;
    //public Transform spawnPoint;
    // Sound Effect by RichardDooDee23 from Pixabay
    public AudioSource audioSource;
    public void SpawnSphere()
    {
        if (poopMeter.poopNum > 0) {
            poopMeter.poopNum--;
            StartCoroutine(ShootPoop());
            //SoundFXManager.PlaySound(SoundType.POOP);
            SoundFXManager.PlaySound3D(SoundType.POOP, transform);
            //audioSource.Play();

            //Instantiate(mySphere, spawnPoint.position, spawnPoint.rotation);
        }
        
    }
    IEnumerator ShootPoop()
    {
        // Start the particle effect

        poopParticles.Play();

        // Wait 
        yield return new WaitForSeconds(shootDuration);

        // Stop emitting new particles
        poopParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
