using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Klasa odpowiedzialna za efekt przyciemnienia i rozjaśnienia ekranu (fade in/out)
public class ScreenFader : MonoBehaviour
{
    // Obiekt UI typu Image, który pokrywa cały ekran i służy jako „czarna zasłona”
    [SerializeField]
    private Image fadeImage;

    // Czas trwania przejścia (fade in/out) w sekundach
    [SerializeField]
    private float fadeDuration = 7.0f;

    // Publiczna metoda do rozpoczęcia efektu przyciemnienia (fade out)
    public void StartFade()
    {
        StartCoroutine(FadeOut());
    }

    // Publiczna metoda do rozpoczęcia efektu rozjaśnienia (fade in)
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    // Korutyna wykonująca efekt Fade Out (rozjaśnia obraz do niemal pełnej czerni)
    private IEnumerator FadeOut()
    {
        float timer = 0f;

        // Kolor początkowy to aktualny kolor obrazu
        Color startColor = fadeImage.color;

        // Kolor końcowy to prawie czarny z wysokim poziomem przezroczystości (0.95f)
        Color endColor = new Color(0f, 0f, 0f, 0.95f);

        // Stopniowo przechodzimy od startColor do endColor przez czas fadeDuration
        while (timer < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Na końcu upewniamy się, że kolor jest ustawiony dokładnie na końcowy
        fadeImage.color = endColor;
    }

    // Korutyna wykonująca efekt Fade In (rozjaśnia obraz z czerni do przezroczystości)
    private IEnumerator FadeIn()
    {
        float timer = 0f;

        // Startujemy od pełnej czerni (alpha = 1)
        Color startColor = new Color(0f, 0f, 0f, 1f);

        // Kończymy na częściowo przezroczystym (alpha = 0.34)
        // Można zmienić na 0f, jeśli chcesz całkowicie rozjaśnić
        Color endColor = new Color(0f, 0f, 0f, 0.34f);

        // Stopniowo zmieniamy kolor z czerni na przezroczystość
        while (timer < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ustawiamy końcowy kolor na 100% pewności
        fadeImage.color = endColor;
    }
}
