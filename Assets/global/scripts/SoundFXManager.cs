using UnityEngine;
using System;

//https://www.youtube.com/watch?v=g5WT91Sn3hg&t=281s
public enum SoundType
{
    FLAP, 
    COO,
    EAT, 
    POOP, 
    SPLAT,
    FOOTSTEP,
    NPC,

}
[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
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
        //instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
        AudioClip clip = instance.GetRandomClip(sound);

        if (clip == null) return;

        instance.audioSource.PlayOneShot(clip, volume);

    }

    public static void PlaySound3D(
        SoundType sound,
        Transform target,
        float volume = 1f)
    {
        if (target == null) return;

        AudioClip clip = instance.GetRandomClip(sound);

        if (clip == null) return;

        // Create a temporary sound object
        GameObject soundObject = new GameObject("3D Sound");

        soundObject.transform.position = target.position;

        // Make the sound follow the target
        soundObject.transform.SetParent(target, true);

        // Add AudioSource
        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;

        // 3D Audio settings
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 2f;
        source.maxDistance = 20f;
        source.dopplerLevel = 0.5f;

        source.Play();

        // Destroy after the sound finishes
        Destroy(soundObject, clip.length);
    }

    // 3D SOUND - Fixed position in the world
    public static void PlaySound3DAtPosition(SoundType sound, Vector3 position, float volume = 1f)
    {
        AudioClip clip = instance.GetRandomClip(sound);

        if (clip == null) return;

        GameObject soundObject = new GameObject("3D Sound");

        soundObject.transform.position = position;

        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;

        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = 2f;
        source.maxDistance = 20f;
        source.dopplerLevel = 0.5f;

        source.Play();

        Destroy(soundObject, clip.length);
    }

    // Get a random clip from the sound list
    private AudioClip GetRandomClip(SoundType sound)
    {
        AudioClip[] clips = soundList[(int)sound].Sounds;

        if (clips == null || clips.Length == 0)
            return null;

        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }

#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}