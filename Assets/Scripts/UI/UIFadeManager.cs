using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFadeManager : MonoBehaviour
{
    public static UIFadeManager Instance { get; private set; }

    private readonly Dictionary<CanvasGroup, Coroutine> _activeFades = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Fade(CanvasGroup group, float targetAlpha, float duration, System.Action onComplete = null)
    {
        if (!group) 
            return;

        if (_activeFades.TryGetValue(group, out Coroutine existing))
            StopCoroutine(existing);

        Coroutine fadeCoroutine = StartCoroutine(FadeRoutine(group, targetAlpha, duration, onComplete));
        _activeFades[group] = fadeCoroutine;
    }

    private IEnumerator FadeRoutine(CanvasGroup group, float targetAlpha, float duration, System.Action onComplete)
    {
        float startAlpha = group.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        group.alpha = targetAlpha;
        onComplete?.Invoke();

        _activeFades.Remove(group);
    }
}
