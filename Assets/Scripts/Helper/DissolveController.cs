using System.Collections;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    Material[] materials;
    const string dissolveProperty = "_Dissolve";

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materials = renderer.materials;
        }
    }

    public void StartDissolveEffect(float targetValue, float duration)
    {
        if (materials == null) return;
        StopAllCoroutines();
        StartCoroutine(DissolveEffect(targetValue, duration));
    }

    IEnumerator DissolveEffect(float targetValue, float duration)
    {
        if (materials == null) yield break;

        float startValue = materials[0].GetFloat(dissolveProperty);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            foreach (var mat in materials)
            {
                mat.SetFloat(dissolveProperty, newValue);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (var mat in materials)
        {
            mat.SetFloat(dissolveProperty, targetValue);
        }
    }
}
