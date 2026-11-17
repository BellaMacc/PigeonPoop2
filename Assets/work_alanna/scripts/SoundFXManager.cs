using UnityEngine;

public enum SoundType
{
    FLAP, 
    COO,
    WIND,
    EAT, 
    POOP, 
    SPLAT,
    FOOTSTEP,
    NPC,

}
[RequireComponent(typeof(AudioSource))]
public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundFXManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public static void PlaySound(SoundType sound, float volume =1)
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}