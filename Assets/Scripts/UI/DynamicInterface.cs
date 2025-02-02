using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{
    [SerializeField] int xStart;
    [SerializeField] int yStart;
    [SerializeField] int xSpaceBetweenItem;
    [SerializeField] int ySpaceBetweenItem;
    [SerializeField] int numberOfColumns;

    public GameObject inventoryPrefab;

    public override void CrateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnEndDrag(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });

            inventory.GetSlots[i].slotDisplay = obj;

            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
        }
    }

    Vector3 GetPosition(int i)
    {
        return new Vector3(xStart + (xSpaceBetweenItem * (i % numberOfColumns)), yStart + ((-ySpaceBetweenItem * (i / numberOfColumns))), 0f);
    }
}
