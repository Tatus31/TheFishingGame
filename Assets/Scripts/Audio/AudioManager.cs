using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static string HeartBeatSound = "HeartBeatSound";

    public static void PlaySound(string soundName)
    {
        GameObject soundObj = GameObject.Find(soundName);
        AudioSource audioSource = soundObj.GetComponent<AudioSource>();
        audioSource.mute = false;
    }

    public static void MuteSound(string soundName)
    {
        GameObject soundObj = GameObject.Find(soundName);
        AudioSource audioSource = soundObj.GetComponent<AudioSource>();
        audioSource.mute = true;
    }
}
