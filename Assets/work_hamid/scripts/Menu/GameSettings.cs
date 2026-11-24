using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

// Stores the three player settings (Sound FX, Music, Render Distance),
// saves them between play sessions with PlayerPrefs, and applies them to the game.
public static class GameSettings
{
    // PlayerPrefs keys (where each value is saved on the computer)
    const string SfxKey = "settings_sfx";
    const string MusicKey = "settings_music";
    const string RenderKey = "settings_render";

    // Volumes are steps from 0 to 10 (shown as 0% - 100%), render distance is 0 / 1 / 2
    public const int VolumeSteps = 10;
    public static readonly string[] RenderNames = { "LOW", "MED", "HIGH" };

    // How much of each camera's original view distance to keep for LOW / MED / HIGH
    static readonly float[] RenderScale = { 0.3f, 0.6f, 1f };

    // Fired whenever a setting changes (MusicVolume listens to this)
    public static event Action Changed;

    // Remembers each camera's original far clip distance so LOW/MED are based on it
    static readonly Dictionary<UnityEngine.Object, float> originalFar = new Dictionary<UnityEngine.Object, float>();

    public static int Sfx
    {
        get => PlayerPrefs.GetInt(SfxKey, 8);
        set { PlayerPrefs.SetInt(SfxKey, Mathf.Clamp(value, 0, VolumeSteps)); Apply(); }
    }

    public static int Music
    {
        get => PlayerPrefs.GetInt(MusicKey, 6);
        set { PlayerPrefs.SetInt(MusicKey, Mathf.Clamp(value, 0, VolumeSteps)); Apply(); }
    }

    public static int Render
    {
        get => PlayerPrefs.GetInt(RenderKey, 2); // default HIGH = the scene's normal view distance
        set { PlayerPrefs.SetInt(RenderKey, Mathf.Clamp(value, 0, RenderNames.Length - 1)); Apply(); }
    }

    // 0-1 volume values for scripts that play audio
    public static float SfxVolume01 => Sfx / (float)VolumeSteps;
    public static float MusicVolume01 => Music / (float)VolumeSteps;

    // Runs automatically when the game starts, and again after every scene load
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Apply();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        originalFar.Clear();
        Apply();
    }

    public static void Apply()
    {
        // SOUND FX: the game only has sound effects for now, so this sets the overall game volume.
        // (Music sources use the MusicVolume component on top of this.)
        AudioListener.volume = SfxVolume01;

        // RENDER DISTANCE: shrink how far the cameras draw
        float scale = RenderScale[Mathf.Clamp(Render, 0, RenderScale.Length - 1)];

        // Plain Unity cameras
        foreach (Camera cam in Camera.allCameras)
            cam.farClipPlane = Original(cam, cam.farClipPlane) * scale;

        // Cinemachine cameras (they override the real camera's far clip, so set them too)
        foreach (CinemachineCamera vcam in UnityEngine.Object.FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            LensSettings lens = vcam.Lens;
            lens.FarClipPlane = Original(vcam, lens.FarClipPlane) * scale;
            vcam.Lens = lens;
        }

        PlayerPrefs.Save();
        Changed?.Invoke();
    }

    // Returns the first far clip value we saw for this camera
    static float Original(UnityEngine.Object key, float current)
    {
        if (!originalFar.TryGetValue(key, out float value))
        {
            value = current;
            originalFar[key] = value;
        }
        return value;
    }
}
