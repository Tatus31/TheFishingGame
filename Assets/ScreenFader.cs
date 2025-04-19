using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 7.0f;
 
    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }
    
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

   

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new Color(0f, 0f, 0f, 0.95f); // Black with alpha 1.
 
        while (timer < fadeDuration)
        {
            // Interpolate the color between start and end over time.
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
 
        // Ensure the image is completely black at the end.
        fadeImage.color = endColor;
    }

     private IEnumerator FadeIn()
    {
        float timer = 0f;
        Color startColor = new Color(0f,0f,0f,1f);
        Color endColor = new Color(0f, 0f, 0f, 0.34f); // Czarny, ale przezroczysty (alpha 0)

        while (timer < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endColor;
    }
   
}