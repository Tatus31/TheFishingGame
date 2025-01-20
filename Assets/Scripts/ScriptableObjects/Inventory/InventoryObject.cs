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
}

[Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[24];
}