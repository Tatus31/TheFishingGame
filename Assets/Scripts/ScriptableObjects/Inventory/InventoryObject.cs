using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InterfaceType
{
    Inventory,
    Equipment
}

[CreateAssetMenu(fileName = "New Inventory", menuName ="Inventory/Create Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabaseObject database;
    public Inventory inventoryContainer;
    public InterfaceType type;
    public InventorySlot[] GetSlots { get { return inventoryContainer.Slots; } }

    public bool AddItem(Item item, int amount, int weight)
    {
        if (EmptySlotCount <= 0)
            return false;

        InventorySlot slot = FindItemOnInventory(item);
        if (!database.GetItem[item.id].stackable || slot == null)
        {
            SetEmptySlot(item, amount, weight);
            return true;
        }
        slot.AddAmount(amount);
        return true;
    }

    public int EmptySlotCount
    {
        get 
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public InventorySlot FindItemOnInventory(Item item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.id == item.id)
            {
                return GetSlots[i];
            }
        }
        return null;
    }

    public InventorySlot SetEmptySlot(Item item, int amount, int weight)
    {
        for(int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.id <= -1)
            {
                GetSlots[i].UpdateSlot(item, amount);
                return GetSlots[i];
            }
        }
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);

            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item == item)
            {
                GetSlots[i].UpdateSlot(null, 0);
            }
        }
    }
}

[Serializable]
public class Inventory
{
    public InventorySlot[] Slots = new InventorySlot[24];

    public void Clear()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].UpdateSlot(new Item(), 0);
        }
    }
}