using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public PlayerAttributes[] playerAttributes;

    private void Start()
    {
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
        var item = other.GetComponent<ItemPhysical>();
        if (item)
        {
            Item _item = new Item(item.item);
            inventory.AddItem(_item, 1, _item.weight);
            Destroy(other.gameObject);
        }
    }

    public void AttributeModified(PlayerAttributes attribute)
    {
        Debug.Log($"{attribute.type} changed to {attribute.value.ModifiedValue} points");
    }

    private void OnApplicationQuit()
    {
        inventory.inventoryContainer.Slots = new InventorySlot[24];
    }
}
