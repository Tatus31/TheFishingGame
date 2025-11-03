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
    public static string SprinklerSound = "SprinklerSound";

    [SerializeField]
    private List<AudioSource> playingSounds = new List<AudioSource>();

    private void Awake()
    {
        if (Instance && Instance != this)
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
        if (!Instance)
        {
#if UNITY_EDITOR
            Debug.LogError("AudioManager instance is null. Cannot play sound.");
#endif
            return;
        }

        StopAllSounds();

        GameObject soundObj = GameObject.Find(soundName);
        
        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            
            if (audioSource)
            {
                audioSource.mute = false;
                audioSource.Play();
                Instance.playingSounds.Add(audioSource);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No AudioSource component found on {soundName}");
#endif
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Sound object '{soundName}' not found in scene");
#endif
        }
    }

    public static void StopAllSounds()
    {
        if (!Instance) 
            return;

        foreach (AudioSource source in Instance.playingSounds)
        {
            if (source)
            {
                source.mute = true;
            }
        }

        Instance.playingSounds.Clear();
    }

    public static void MuteSound(string soundName, float fadeDuration = 1f)
    {
        if (!Instance) 
            return;

        GameObject soundObj = GameObject.Find(soundName);

        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();

            if (audioSource)
            {
                Instance.StartCoroutine(FadeOutAndMute(audioSource, fadeDuration));
            }
        }
    }

    private static IEnumerator FadeOutAndMute(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.mute = true;

        Instance.playingSounds.Remove(audioSource);
    }
    
    public static void UnmuteSound(string soundName)
    {
        if (!Instance) 
            return;
        
        GameObject soundObj = GameObject.Find(soundName);
        
        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            
            if (audioSource)
            {
                audioSource.mute = false;
                audioSource.volume = 1f;
                if (!Instance.playingSounds.Contains(audioSource))
                {
                    Instance.playingSounds.Add(audioSource);
                }
            }
        }
    }

    public static void ChangeAudioVolume(string soundName, float volume)
    {
        GameObject soundObj = GameObject.Find(soundName);
        
        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            
            if (audioSource)
            {
                audioSource.volume = Mathf.Clamp01(volume);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No AudioSource component found on {soundName}");
#endif
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Sound object '{soundName}' not found in scene");
#endif
        }
    }

    public static void ChangeAudioPitch(string soundName, float pitch)
    {
        GameObject soundObj = GameObject.Find(soundName);
        
        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            
            if (audioSource)
            {
                audioSource.pitch = pitch;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No AudioSource component found on {soundName}");
#endif
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Sound object '{soundName}' not found in scene");
#endif
        }
    }

    public static bool IsSoundPlaying(string soundName)
    {
        if (!Instance)
            return false;

        GameObject soundObj = GameObject.Find(soundName);
        
        if (soundObj)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            if (audioSource)
            {
                return audioSource.isPlaying && !audioSource.mute;
            }
        }

        return false;
    }
}