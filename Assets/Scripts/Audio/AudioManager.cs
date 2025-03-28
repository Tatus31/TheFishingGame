using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static string HeartBeatSound = "HeartBeatSound";
    public static string HeartBeatSlowSound = "HeartBeatSlowSound";

    public static void PlaySound(string soundName)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.mute = false;
        }
    }

    public static void MuteSound(string soundName)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.mute = true;
        }
    }

    public static void ChangeAudioVolume(string soundName, float volume)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if(soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.volume = volume;
        }
    }

    public static void FadeOutSound(string soundName)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
            if (audioSource.volume <= 0)
            {
                audioSource.mute = true;
            }
        }
    }

    public static void FadeInSound(string soundName, float volumeBefore)
    {
        GameObject soundObj = GameObject.Find(soundName);
        if (soundObj != null)
        {
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.volume = Mathf.Lerp(audioSource.volume, volumeBefore, Time.deltaTime);
            if (audioSource.volume >= volumeBefore)
            {
                audioSource.mute = false;
            }
        }
    }
}
