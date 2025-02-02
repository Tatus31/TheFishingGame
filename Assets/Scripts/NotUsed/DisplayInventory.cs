//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;


//public class DisplayInventory : MonoBehaviour
//{
//    public GameObject inventoryPrefab;
//    public InventoryObject inventory;

//    [SerializeField] int xStart;
//    [SerializeField] int yStart;
//    [SerializeField] int xSpaceBetweenItem;
//    [SerializeField] int ySpaceBetweenItem;
//    [SerializeField] int numberOfColumns;

//    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

//    private void Start()
//    {
//        CrateSlots();
//    }

//    private void Update()
//    {
//        UpdateSlots();
//    }

//    public void UpdateSlots()
//    {
//        foreach (KeyValuePair<GameObject, InventorySlot> slot in itemsDisplayed)
//        {
//            if(slot.Value.item.id >= 0)
//            {
//                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.Value.item.id].uiDisplay;
//                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1,1,1,1);
//                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = slot.Value.amount == 1 ? "" : slot.Value.amount.ToString("n0");
//            }
//            else
//            {
//                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
//                slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
//                slot.Key.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
//            }
//        }
//    }

//    public void CrateSlots()
//    {
//        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
//        for (int i = 0; i < inventory.inventoryContainer.Items.Length; i++)
//        {
//            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
//            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

//            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
//            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
//            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
//            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
//            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

//            itemsDisplayed.Add(obj, inventory.inventoryContainer.Items[i]);
//        }
//    }

//    void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
//    {
//        EventTrigger trigger = obj.GetComponent<EventTrigger>();
//        var eventTrigger = new EventTrigger.Entry();
//        eventTrigger.eventID = type;
//        eventTrigger.callback.AddListener(action);
//        trigger.triggers.Add(eventTrigger);
//    }

//    public void OnEnter(GameObject obj)
//    {
//        MouseData.slotHoveredOver = obj;
//    }

//    public void OnExit(GameObject obj)
//    {
//        MouseData.slotHoveredOver = null;
//    }

//    public void OnDragStart(GameObject obj)
//    {
//        var mouseObject = new GameObject();
//        var rectTransform = mouseObject.AddComponent<RectTransform>();
//        rectTransform.sizeDelta = new Vector2(75,75);
//        mouseObject.transform.SetParent(transform.parent);
//        if (itemsDisplayed[obj].item.id >= 0)
//        {
//            var image = mouseObject.AddComponent<Image>();
//            image.sprite = inventory.database.GetItem[itemsDisplayed[obj].item.id].uiDisplay;
//            image.raycastTarget = false;
//        }
//        mouseItem.tempItemDragged = mouseObject;
//        mouseItem.item = itemsDisplayed[obj];
//    }

//    public void OnEndDrag(GameObject obj)
//    {
//        if (mouseItem.slotHoveredOver)
//        {
//            inventory.SwapItems(itemsDisplayed[obj], itemsDisplayed[mouseItem.slotHoveredOver]);
//        }
//        else
//        {
//            inventory.RemoveItem(itemsDisplayed[obj].item);
//        }

//        Destroy(mouseItem.tempItemDragged);
//        mouseItem.item = null;
//    }

//    public void OnDrag(GameObject obj)
//    {
//        if(mouseItem.tempItemDragged != null)
//        {
//            mouseItem.tempItemDragged.GetComponent<RectTransform>().position = Input.mousePosition;
//        }
//    }

//    public Vector3 GetPosition(int i)
//    {
//        return new Vector3(xStart + (xSpaceBetweenItem * (i % numberOfColumns)), yStart + ((-ySpaceBetweenItem * (i/ numberOfColumns))), 0f);
//    }
//}

