using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public static string HeartBeatSound = "HeartBeatSound";
    public static string HeartBeatSlowSound = "HeartBeatSlowSound";
    public static string WalkSound = "Audio";

    [SerializeField]
    private List<AudioSource> playingSounds = new List<AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Multiple instances of AudioManager found. Destroying duplicate on {gameObject.name}");
#endif
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static void PlaySound(string soundName)
    {
        if (Instance == null)
        {
            Debug.LogError("AudioManager instance is null. Cannot play sound.");
            return;
        }

        StopAllSounds();

        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.mute = false;
                audioSource.Play();
                Instance.playingSounds.Add(audioSource);
            }
            else
            {
                Debug.LogWarning($"No AudioSource component found on {soundName}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound object '{soundName}' not found in scene");
        }
    }

    public static void StopAllSounds()
    {
        if (Instance == null) return;

        foreach (AudioSource source in Instance.playingSounds)
        {
            if (source != null)
            {
                source.mute = true;
            }
        }

        Instance.playingSounds.Clear();
    }

    public static void MuteSound(string soundName)
    {
        if (Instance == null) return;

        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.mute = true;
                Instance.playingSounds.Remove(audioSource);
            }
        }
    }

    public static void ChangeAudioVolume(string soundName, float volume)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = Mathf.Clamp01(volume);
            }
            else
            {
                Debug.LogWarning($"No AudioSource component found on {soundName}");
            }
        }
        else
        {
            Debug.LogWarning($"Sound object '{soundName}' not found in scene");
        }
    }

    public static bool IsSoundPlaying(string soundName)
    {
        if (Instance == null) return false;

        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                return audioSource.isPlaying && !audioSource.mute;
            }
        }

        return false;
    }
}