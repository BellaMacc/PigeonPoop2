using UnityEngine;
using System;
// https://www.youtube.com/watch?v=g5WT91Sn3hg&t=281s
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
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
        
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