using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    public int id = -1;
    public Item item;
    public int amount;


    public InventorySlot()
    {
        this.id = -1;
        this.item = null;
        this.amount = 0;
    }

    public InventorySlot(int id, Item item, int amount) 
    {
        this.id = id;
        this.item = item;
        this.amount = amount;
    }

    public void AddAmount(int amount)
    {
        this.amount += amount;
    }

    public void UpdateSlot(int id, Item item, int amount)
    {
        this.id = id;
        this.item = item;
        this.amount = amount;
    }
}
