using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName ="Inventory/Create Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabaseObject database;
    public Inventory inventoryContainer;

    public void AddItem(Item item, int amount)
    {
        for (int i = 0; i < inventoryContainer.Items.Length ; i++)
        {
            if (inventoryContainer.Items[i].id == item.id)
            {
                inventoryContainer.Items[i].AddAmount(amount);
                return;
            }
        }
        SetEmptySlot(item, amount);
    }

    public InventorySlot SetEmptySlot(Item item, int amount)
    {
        for(int i = 0; i < inventoryContainer.Items.Length; i++)
        {
            if (inventoryContainer.Items[i].id <= -1)
            {
                inventoryContainer.Items[i].UpdateSlot(item.id, item, amount);
                return inventoryContainer.Items[i];
            }
        }
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.id, item2.item, item2.amount);
        item2.UpdateSlot(item1.id, item1.item, item1.amount);
        item1.UpdateSlot(temp.id, temp.item, temp.amount);
    }

    public void RemoveItem(Item item)
    {
        for (int i = 0; i < inventoryContainer.Items.Length; i++)
        {
            if (inventoryContainer.Items[i].item == item)
            {
                inventoryContainer.Items[i].UpdateSlot(-1, null, 0);
            }
        }
    }
}

[Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[24];
}