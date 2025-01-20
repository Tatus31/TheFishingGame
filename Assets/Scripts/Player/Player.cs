using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryObject inventory;

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<ItemPhysical>();
        if (item)
        {
            Item _item = new Item(item.item);
            inventory.AddItem(_item, 1);
            Destroy(other.gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        inventory.inventoryContainer.Items = new InventorySlot[24];
    }
}
