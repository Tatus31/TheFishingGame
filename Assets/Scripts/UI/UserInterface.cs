using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class UserInterface : MonoBehaviour
{
    public Player player;
    public InventoryObject inventory;
    protected Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    GameObject tooltipPanel;
    TextMeshProUGUI itemNameText;
    TextMeshProUGUI itemDescriptionText;

    float tooltipOffset = -115f;

    Coroutine fadeCoroutine;
    GameObject currentHoveredObj;

    private void Start()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }
        CrateSlots();
        CreateTooltip();

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
    }

    void CreateTooltip()
    {
        tooltipPanel = new GameObject("ItemTooltip");
        tooltipPanel.transform.SetParent(transform.parent);
        RectTransform panelRect = tooltipPanel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(200, 100);

        Image panelImage = tooltipPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        VerticalLayoutGroup layout = tooltipPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 5;

        GameObject nameObj = new GameObject("ItemName");
        nameObj.transform.SetParent(tooltipPanel.transform);
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.sizeDelta = new Vector2(180, 30);
        itemNameText = nameObj.AddComponent<TextMeshProUGUI>();
        itemNameText.fontSize = 16;
        itemNameText.fontStyle = FontStyles.Bold;
        itemNameText.color = Color.white;
        itemNameText.alignment = TextAlignmentOptions.Left;

        GameObject descObj = new GameObject("ItemDescription");
        descObj.transform.SetParent(tooltipPanel.transform);
        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.sizeDelta = new Vector2(180, 60);
        itemDescriptionText = descObj.AddComponent<TextMeshProUGUI>();
        itemDescriptionText.fontSize = 12;
        itemDescriptionText.color = Color.white;
        itemDescriptionText.alignment = TextAlignmentOptions.Left;

        tooltipPanel.SetActive(false);

        CanvasGroup canvasGroup = tooltipPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void OnSlotUpdate(InventorySlot slot)
    {
        if (slot.item.id >= 0)
        {
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = slot.ItemObject.uiDisplay;
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            slot.slotDisplay.transform.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount == 1 ? "" : slot.amount.ToString("n0");
        }
        else
        {
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
            slot.slotDisplay.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    private void Update()
    {
        UpdateSlots();

        if (tooltipPanel.activeSelf)
        {
            UpdateTooltipPosition();
        }
    }

    void UpdateTooltipPosition()
    {
        Vector2 position = Input.mousePosition;
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        Canvas rootCanvas = transform.root.GetComponent<Canvas>();
        if (rootCanvas != null)
        {
            RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

            float rightEdgeToScreen = Screen.width - (position.x + tooltipOffset + tooltipRect.rect.width);
            float bottomEdgeToScreen = position.y - tooltipRect.rect.height;

            if (rightEdgeToScreen < 0)
                position.x += rightEdgeToScreen;

            if (bottomEdgeToScreen < 0)
                position.y = tooltipRect.rect.height;
        }

        tooltipRect.position = new Vector2(position.x + tooltipOffset, position.y);
    }

    void ShowTooltip(InventorySlot slot)
    {
        if (slot != null && slot.item.id >= 0)
        {
            itemNameText.text = slot.ItemObject.name;
            itemDescriptionText.text = slot.ItemObject.description;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            tooltipPanel.SetActive(true);
            fadeCoroutine = StartCoroutine(FadeTooltip(true));
        }
    }

    void HideTooltip()
    {
        if (tooltipPanel.activeSelf)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeTooltip(false));
        }
    }

    IEnumerator FadeTooltip(bool fadeIn)
    {
        CanvasGroup canvasGroup = tooltipPanel.GetComponent<CanvasGroup>();
        float targetAlpha = fadeIn ? 1f : 0f;
        float startAlpha = canvasGroup.alpha;
        float duration = 0.1f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (!fadeIn && canvasGroup.alpha <= 0)
        {
            tooltipPanel.SetActive(false);
        }

        fadeCoroutine = null;
    }

    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> slot in slotsOnInterface)
        {
            if (slot.Value.item.id >= 0)
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = slot.Value.ItemObject.uiDisplay;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.amount == 1 ? "" : slot.Value.amount.ToString("n0");
            }
            else
            {
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    public abstract void CrateSlots();

    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
        currentHoveredObj = obj;

        if (slotsOnInterface.ContainsKey(obj) && slotsOnInterface[obj].item.id >= 0)
        {
            ShowTooltip(slotsOnInterface[obj]);
        }
    }

    public void OnExit(GameObject obj)
    {
        MouseData.slotHoveredOver = null;

        if (currentHoveredObj == obj)
        {
            currentHoveredObj = null;
            HideTooltip();
        }
    }

    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }

    public void OnDragStart(GameObject obj)
    {
        currentHoveredObj = null;
        HideTooltip();

        var mouseObject = new GameObject();
        var rectTransform = mouseObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(75, 75);
        mouseObject.transform.SetParent(transform.parent);
        if (slotsOnInterface[obj].item.id >= 0)
        {
            var image = mouseObject.AddComponent<Image>();
            image.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
            image.raycastTarget = false;
        }

        MouseData.tempItemDragged = mouseObject;
    }

    public void OnEndDrag(GameObject obj)
    {
        Destroy(MouseData.tempItemDragged);

        if (MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            return;
        }

        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];

            if (mouseHoverSlotData.CanPlaceInSlot(slotsOnInterface[obj].ItemObject) &&
                slotsOnInterface[obj].CanPlaceInSlot(mouseHoverSlotData.ItemObject))
            {
                Item tempItem = mouseHoverSlotData.item;
                int tempAmount = mouseHoverSlotData.amount;

                mouseHoverSlotData.UpdateSlot(slotsOnInterface[obj].item, slotsOnInterface[obj].amount);
                slotsOnInterface[obj].UpdateSlot(tempItem, tempAmount);
            }
        }
    }

    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemDragged != null)
        {
            MouseData.tempItemDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}

public static class MouseData
{
    public static GameObject tempItemDragged;
    public static GameObject slotHoveredOver;
    public static UserInterface interfaceMouseIsOver;
}