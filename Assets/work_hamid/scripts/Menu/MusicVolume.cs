using UnityEngine;

// Put this on any AudioSource that plays MUSIC.
// It keeps that source's volume in sync with the MUSIC slider in the Options menu.
[RequireComponent(typeof(AudioSource))]
public class MusicVolume : MonoBehaviour
{
    [Tooltip("Volume this music plays at when the MUSIC slider is at 100%")]
    [Range(0f, 1f)] public float fullVolume = 1f;

    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GameSettings.Changed += UpdateVolume;
        UpdateVolume();
    }

    void OnDisable()
    {
        GameSettings.Changed -= UpdateVolume;
    }

    void UpdateVolume()
    {
        if (source != null)
            source.volume = fullVolume * GameSettings.MusicVolume01;
    }
}
