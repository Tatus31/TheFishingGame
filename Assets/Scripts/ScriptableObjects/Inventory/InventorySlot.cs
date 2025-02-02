using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public delegate void SlotUpdated(InventorySlot slot);

[Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [NonSerialized]
    public UserInterface parent;
    [NonSerialized]
    public GameObject slotDisplay;
    [NonSerialized]
    public SlotUpdated OnAfterUpdate;
    [NonSerialized]
    public SlotUpdated OnBeforeUpdate;
    public Item item;
    public int amount;
    public ItemObject ItemObject 
    { 
        get 
        {
            if (item.id >= 0) 
                return parent.inventory.database.GetItem[item.id]; 
            return null;
        } 
    }

    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }

    public InventorySlot(Item item, int amount) 
    {
        UpdateSlot(item, amount);
    }

    public void AddAmount(int amount)
    {
        UpdateSlot(item, this.amount += amount);
    }

    public void UpdateSlot(Item item, int amount)
    {
        if(OnBeforeUpdate != null)
        {
            OnBeforeUpdate.Invoke(this);
        }

        this.item = item;
        this.amount = amount;

        if (OnAfterUpdate != null)
        {
            OnAfterUpdate.Invoke(this);
        }
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }

    public bool CanPlaceInSlot(ItemObject itemObject)
    {
        if (AllowedItems.Length <= 0 || itemObject == null || itemObject.data.id < 0)
        {
            return true;
        }

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if(itemObject.type == AllowedItems[i])
            {
                return true;
            }
        }
        return false;
    }
}
