using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PopupManager : MonoBehaviour
{
    [SerializeField] PopupSO[] popupsSO;
    [SerializeField] GameObject panelPrefab;

    [SerializeField] float hideDelay = 0.5f;

    LayerMask hitLayerMask;

    TextMeshProUGUI infoText;
    TextMeshProUGUI interactText;

    Coroutine hidePanelCoroutine;

    private void Start()
    {
        if (popupsSO == null || popupsSO.Length <= 0)
        {
            Debug.LogError("PopupSO is not assigned in PopupManager.");
        }

        if (panelPrefab == null)
        {
            Debug.LogError("Panel Prefab is not assigned in PopupManager.");
        }

        InfoKey infoKey = panelPrefab.GetComponentInChildren<InfoKey>(true);
        InteractKey interactKey = panelPrefab.GetComponentInChildren<InteractKey>(true);

        if (infoKey)
            infoText = infoKey.GetComponent<TextMeshProUGUI>();

        if (interactKey)
            interactText = interactKey.GetComponent<TextMeshProUGUI>();

        panelPrefab.SetActive(true);
        InitializePopup(popupsSO[0]);
        Canvas.ForceUpdateCanvases();
        panelPrefab.SetActive(false);

        foreach (var popup in popupsSO)
        {
            popup.IsInteractionActive = false;
        }
    }

    private void Update()
    {
        if (MouseWorldPosition.GetInteractable(MouseWorldPosition.Instance.InteractableMask, out hitLayerMask))
        {
            foreach (var popup in popupsSO)
            {
                if (popup.InteractableLayer == hitLayerMask)
                {
                    if (!panelPrefab.activeInHierarchy)
                    {
                        InitializePopup(popup);
                    }
                }
            }

            if (hidePanelCoroutine != null)
            {
                StopCoroutine(hidePanelCoroutine);
                hidePanelCoroutine = null;
            }
        }
        else
        {
            if (panelPrefab.activeInHierarchy && hidePanelCoroutine == null)
            {
                hidePanelCoroutine = StartCoroutine(HidePanelAfterDelay());
            }
        }
    }

    void InitializePopup(PopupSO popup)
    {
        if (popup.IsInteractionActive)
            return;

        panelPrefab.SetActive(true);

        if (infoText != null)
            infoText.text = popup.InformationKey.ToString();

        if (interactText != null)
            interactText.text = popup.InteractionKey.ToString();

        popup.IsInteractionActive = true;
    }

    IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        foreach (var popup in popupsSO)
        {
            popup.IsInteractionActive = false;
        }

        panelPrefab.SetActive(false);
        hidePanelCoroutine = null;
    }
}
