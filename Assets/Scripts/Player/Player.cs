using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public PlayerAttributes[] playerAttributes;

    Compass compass;

    private void Start()
    {
        if (compass == null)
        {
            compass = FindObjectOfType<Compass>();
        }

        for (int i = 0; i < playerAttributes.Length; i++)
        {
            playerAttributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public void OnRemoveItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < playerAttributes.Length; j++)
                    {
                        if (playerAttributes[j].type == slot.item.stats[i].stats)
                            playerAttributes[j].value.RemoveModifier(slot.item.stats[i]);
                    }
                }

                break;
            default:
                break;
        }
    }

    public void OnAddItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < playerAttributes.Length; j++)
                    {
                        if (playerAttributes[j].type == slot.item.stats[i].stats)
                            playerAttributes[j].value.AddModifier(slot.item.stats[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var itemObj = other.GetComponent<ItemPhysical>();
        if (itemObj)
        {
            Item _item = new Item(itemObj.item);
            inventory.AddItem(_item, 1, _item.weight);

            var marker = other.GetComponent<Marker>();
            if (marker != null && compass != null)
            {
                compass.DeleteMarker(marker);
            }

            var analiticsData = new Dictionary<string, object>
            {
                { "pickedUpItem", itemObj.item.data.name },
                { "isQuestItem", itemObj.item.data.isQuestItem }
            };

            if (itemObj.item.data.isQuestItem)
            {
                AnalyticsEvents.SendAnalyticsEvent("OnQuestItemPickUp", analiticsData);
            }
            else
            {
                AnalyticsEvents.SendAnalyticsEvent("OnItemPickUp", analiticsData);
            }

            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //var monster = collision.gameObject.GetComponent<MonsterLargeStateMachine>();

        //if (monster)
        //{
        //    LeaveUnderDeck.Instance.MovePlayerManually();
        //}
    }

    public void AttributeModified(PlayerAttributes attribute)
    {
//#if UNITY_EDITOR
//        Debug.Log($"{attribute.type} changed to {attribute.value.ModifiedValue} points");
//#endif
    }

    private void OnApplicationQuit()
    {
        inventory.inventoryContainer.Slots = new InventorySlot[24];
    }
}
