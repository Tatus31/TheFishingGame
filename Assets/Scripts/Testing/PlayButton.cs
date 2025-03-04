using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlaySoundOnClick : MonoBehaviour
{
    public AudioSource audioSource; // Referencja do AudioSource
    public AudioClip clickSound; // Dźwięk do odtworzenia
    private bool isPlaying = false;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleSound);
        }
    }

    void ToggleSound()
    {
        if (audioSource != null && clickSound != null)
        {
            if (isPlaying)
            {
                audioSource.Stop();
                isPlaying = false;
            }
            else
            {
                audioSource.PlayOneShot(clickSound);
                isPlaying = true;
            }
        }
    }
}