using TMPro;
using UnityEngine;

public class InfoPanelManager : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;
    [SerializeField] float fadeDuration = 0.5f;

    PopupSO currentPopup;

    CanvasGroup infoCanvasGroup;

    bool wasLookingAtInteractable = false;

    private void Start()
    {
        PopupManager.OnPopupActivated += PopupManager_OnPopupActivated;

        if (infoPanel == null)
            Debug.LogError("Info Panel is not assigned in InfoPanelManager.");

        infoPanel.SetActive(false);

        if (infoPanel.TryGetComponent(out CanvasGroup group))
            infoCanvasGroup = group;
        else
            Debug.LogError("CanvasGroup component not found on Info Panel.");
    }

    private void PopupManager_OnPopupActivated(PopupSO popup)
    {
        currentPopup = popup;
    }

    void ToggleInfoPanel(PopupSO popup)
    {
        if (currentPopup == null || infoCanvasGroup == null)
            return;

        bool isActive = infoPanel.activeSelf;

        if (!isActive)
        {
            UIFadeManager.Instance.Fade(infoCanvasGroup, 1f, fadeDuration);
            infoPanel.SetActive(true);
        }
        else
        {
            UIFadeManager.Instance.Fade(infoCanvasGroup, 0f, fadeDuration, () => infoPanel.SetActive(false));
        }
    }

    void TurnOffInfoPanel()
    {
        UIFadeManager.Instance.Fade(infoCanvasGroup, 0f, fadeDuration, () => infoPanel.SetActive(false));
    }

    void SetTextInChild(string text)
    {
        var textComponent = infoPanel.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }

    private void Update()
    {
        if (currentPopup == null)
            return;

        bool isLookingAtInteractable = MouseWorldPosition.GetInteractable(MouseWorldPosition.Instance.InteractableMask);

        if (wasLookingAtInteractable && !isLookingAtInteractable && infoPanel.activeSelf)
        {
            TurnOffInfoPanel();
        }

        wasLookingAtInteractable = isLookingAtInteractable;

        if (isLookingAtInteractable && Input.GetKeyDown(currentPopup.InformationKey))
        {
            ToggleInfoPanel(currentPopup);
            SetTextInChild(currentPopup.InfoText);
        }
    }
}
