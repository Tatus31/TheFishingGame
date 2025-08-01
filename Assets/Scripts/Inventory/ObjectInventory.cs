using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public abstract class ObjectInventory : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;

    public ObjectAttributes[] objectAttributes;

    protected virtual void Start()
    {
        for (int i = 0; i < objectAttributes.Length; i++)
        {
            objectAttributes[i].SetParent(this);
        }
        for (int i = 0; i < equipment.GetSlots.Length; i++)
        {
            equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
            equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public virtual void OnRemoveItem(InventorySlot slot)
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
                    for (int j = 0; j < objectAttributes.Length; j++)
                    {
                        if (objectAttributes[j].type == slot.item.stats[i].stats)
                            objectAttributes[j].Value.RemoveModifier(slot.item.stats[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    public virtual void OnAddItem(InventorySlot slot)
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
                    for (int j = 0; j < objectAttributes.Length; j++)
                    {
                        if (objectAttributes[j].type == slot.item.stats[i].stats)
                            objectAttributes[j].Value.AddModifier(slot.item.stats[i]);
                    }
                }
                break;
            default:
                break;
        }
    }

    public virtual int GetModifiedStatValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.Value.ModifiedValue;
            }
        }

        return 0;
    }

    public virtual int GetStatBaseValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.Value.BaseValue;
            }
        }

        return 0;
    }

    public void AttributeModified(ObjectAttributes attribute)
    {
        Debug.Log($"{attribute.type} changed to {attribute.Value.ModifiedValue} points");
    }
}
