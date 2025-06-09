using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPopupWindow : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 3f;

    GameObject currentHoveredObject = null;
    ShipMovement shipMovement;

    Dictionary<GameObject, Coroutine> fadeCoroutines = new Dictionary<GameObject, Coroutine>();

    private void Start()
    {
        shipMovement = FindObjectOfType<ShipMovement>();
    }

    void Update() 
    {
        GameObject objCanv = MouseWorldPosition.GetObjectOverMouse(MouseWorldPosition.Instance.InteractableMask);

        if (currentHoveredObject != objCanv)
        {
            if (currentHoveredObject != null)
                FadeOutInteractableUI(currentHoveredObject);

            if (objCanv != null)
                FadeInInteractableUI(objCanv);

            currentHoveredObject = objCanv;
        }
    }

    private void FadeInInteractableUI(GameObject obj)
    {
        Canvas canvas = obj.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            GameObject interactObj = canvas.gameObject;
            GameObject uiElement = interactObj.transform.GetChild(0).gameObject;

            bool shouldShowUI = MouseWorldPosition.GetInteractable(MouseWorldPosition.Instance.InteractableMask);

            if (shouldShowUI)
            {
                StopFadeCoroutine(uiElement);
                uiElement.SetActive(true);

                Coroutine fadeCoroutine = StartCoroutine(FadeUI(uiElement, 0f, 1f));
                fadeCoroutines[uiElement] = fadeCoroutine;
            }
        }
    }

    private void FadeOutInteractableUI(GameObject obj)
    {
        if (obj == null) return;

        Canvas canvas = obj.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            GameObject uiElement = canvas.transform.GetChild(0).gameObject;

            if (uiElement.activeInHierarchy)
            {
                StopFadeCoroutine(uiElement);

                Coroutine fadeCoroutine = StartCoroutine(FadeUI(uiElement, null, 0f, true));
                fadeCoroutines[uiElement] = fadeCoroutine;
            }
        }
    }

    private void StopFadeCoroutine(GameObject uiElement)
    {
        if (fadeCoroutines.ContainsKey(uiElement) && fadeCoroutines[uiElement] != null)
        {
            StopCoroutine(fadeCoroutines[uiElement]);
            fadeCoroutines.Remove(uiElement);
        }
    }

    private IEnumerator FadeUI(GameObject uiElement, float? startAlpha, float targetAlpha, bool deactivateOnComplete = false)
    {
        Graphic[] graphics = uiElement.GetComponentsInChildren<Graphic>();

        if (graphics.Length == 0) yield break;

        if (startAlpha.HasValue)
        {
            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a = startAlpha.Value;
                graphic.color = color;
            }
        }

        float currentAlpha = graphics[0].color.a;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * fadeSpeed;
            float alpha = Mathf.Lerp(currentAlpha, targetAlpha, elapsedTime);

            foreach (Graphic graphic in graphics)
            {
                Color color = graphic.color;
                color.a = alpha;
                graphic.color = color;
            }

            yield return null;
        }

        foreach (Graphic graphic in graphics)
        {
            Color color = graphic.color;
            color.a = targetAlpha;
            graphic.color = color;
        }

        if (deactivateOnComplete && targetAlpha <= 0f)
        {
            uiElement.SetActive(false);
        }

        if (fadeCoroutines.ContainsKey(uiElement))
        {
            fadeCoroutines.Remove(uiElement);
        }
    }
}
