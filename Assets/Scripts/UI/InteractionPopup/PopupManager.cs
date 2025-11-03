using System.Collections;
using UnityEngine;
using TMPro;
using Game;

public class PopupManager : MonoBehaviour
{
    public static event System.Action<PopupSO> OnPopupActivated;

    [SerializeField] PopupSO[] popupsSO;
    [SerializeField] GameObject panelPrefab;
    [SerializeField] float fadeDuration = 0.5f;

    LayerMask currentLayerMask = -1;
    Coroutine hidePanelCoroutine;

    TextMeshProUGUI infoText;
    TextMeshProUGUI interactText;
    CanvasGroup panelCanvasGroup;

    bool cantShowPanel = false;

    private void Start()
    {
        if (popupsSO == null || popupsSO.Length == 0)
        {
#if UNITY_EDITOR
            Debug.LogError("PopupSO is not assigned in PopupManager.");
#endif
        }


        if (panelPrefab == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Panel Prefab is not assigned in PopupManager.");
#endif
        }

        InfoKey infoKey = panelPrefab.GetComponentInChildren<InfoKey>(true);
        InteractKey interactKey = panelPrefab.GetComponentInChildren<InteractKey>(true);

        if (infoKey)
            infoText = infoKey.GetComponent<TextMeshProUGUI>();

        if (interactKey)
            interactText = interactKey.GetComponent<TextMeshProUGUI>();

        if (panelPrefab.TryGetComponent<CanvasGroup>(out CanvasGroup canvasGroup))
        {
            panelCanvasGroup = canvasGroup;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("CanvasGroup component not found on panelPrefab.");
#endif
        }

        EventManager.AddListener<OnEnteredInteraction>(OnEnteredInteraction);

        panelPrefab.SetActive(false);
        panelCanvasGroup.alpha = 0f;

        foreach (var popup in popupsSO)
            popup.IsInteractionActive = false;
    }

    void OnEnteredInteraction(OnEnteredInteraction e)
    {
        cantShowPanel = e.CanShowPanel;
        StartCoroutine(HidePanelAfterDelay());
    }

    private void Update()
    {
        if (cantShowPanel)
            return;

        if (MouseWorldPosition.GetInteractable(MouseWorldPosition.Instance.InteractableMask, out LayerMask hitLayerMask))
        {
            if (hitLayerMask != currentLayerMask || !panelPrefab.activeSelf)
            {
                currentLayerMask = hitLayerMask;

                foreach (var popup in popupsSO)
                {
                    if (popup.InteractableLayer == hitLayerMask)
                    {
                        InitializePopup(popup);
                        break;
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
        foreach (var p in popupsSO)
            p.IsInteractionActive = false;

        popup.IsInteractionActive = true;

        if (!panelPrefab.activeSelf)
            panelPrefab.SetActive(true);

        panelCanvasGroup.alpha = 0f;

        if (infoText)
            infoText.text = popup.InformationKey.ToString();

        if (interactText)
            interactText.text = popup.InteractionKey.ToString();

        OnPopupActivated?.Invoke(popup);

        UIFadeManager.Instance.Fade(panelCanvasGroup, 1f, fadeDuration);
    }

    IEnumerator HidePanelAfterDelay()
    {
        UIFadeManager.Instance.Fade(panelCanvasGroup, 0f, fadeDuration, () =>{ panelPrefab.SetActive(false); });

        yield return new WaitForSeconds(fadeDuration);

        foreach (var popup in popupsSO)
            popup.IsInteractionActive = false;

        currentLayerMask = -1;
        hidePanelCoroutine = null;
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<OnEnteredInteraction>(OnEnteredInteraction);
    }
}
