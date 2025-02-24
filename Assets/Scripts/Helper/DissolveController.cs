using System.Collections;
using UnityEngine;

public static class DissolveController
{
    public static void StartDissolveEffect(GameObject target, float dissolveStart, float dissolveDuration)
    {
        if (target == null)
        {
            return;
        }

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer == null || renderer.material == null)
        {
            return;
        }

        Material material = renderer.material;
        target.SetActive(true);

        target.GetComponent<MonoBehaviour>().StartCoroutine(DissolveRoutine(material, dissolveStart, dissolveDuration));
    }

    private static IEnumerator DissolveRoutine(Material material, float dissolveStart, float dissolveDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float dissolveAmount = Mathf.Lerp(dissolveStart, 1f, elapsedTime / dissolveDuration);
            material.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }
    }
}
