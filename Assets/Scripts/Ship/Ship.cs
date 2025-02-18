using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : ObjectInventory
{
    //public ShipAttributes[] shipAttributes;

    [Header("Basic Ship Parts")]
    [SerializeField] GameObject basicHull;
    //[SerializeField] GameObject basicPropeller;
    //[SerializeField] GameObject basicStorage;
    //[SerializeField] GameObject basicDetection;
    //[SerializeField] GameObject basicNavigation;

    GameObject currentHullDisplay;
    GameObject currentPropellerDisplay;
    GameObject currentStorageDisplay;
    GameObject currentDetectionDisplay;
    GameObject currentNavigationDisplay;

    //private void Start()
    //{
    //    for (int i = 0; i < shipAttributes.Length; i++)
    //    {
    //        shipAttributes[i].SetParent(this);
    //    }
    //    for (int i = 0; i < equipment.GetSlots.Length; i++)
    //    {
    //        equipment.GetSlots[i].OnBeforeUpdate += OnRemoveItem;
    //        equipment.GetSlots[i].OnAfterUpdate += OnAddItem;
    //    }
    //}

    public override void OnRemoveItem(InventorySlot slot)
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
                            objectAttributes[j].value.RemoveModifier(slot.item.stats[i]);
                    }
                }

                switch (slot.AllowedItems[0])
                {
                    case ItemType.Hull:
                        if (currentHullDisplay != null)
                        {
                            Destroy(currentHullDisplay);
                            currentHullDisplay = null;
                            basicHull.SetActive(true);
                        }
                        break;
                    case ItemType.Propeller:
                        if (currentPropellerDisplay != null)
                        {
                            Destroy(currentPropellerDisplay);
                            currentPropellerDisplay = null;
                            //basicPropeller.SetActive(true);
                        }
                        break;
                    case ItemType.Storage:
                        if (currentStorageDisplay != null)
                        {
                            Destroy(currentStorageDisplay);
                            currentStorageDisplay = null;
                            //basicStorage.SetActive(true);
                        }
                        break;
                    case ItemType.Detection:
                        if (currentDetectionDisplay != null)
                        {
                            Destroy(currentDetectionDisplay);
                            currentDetectionDisplay = null;
                            //basicDetection.SetActive(true);
                        }
                        break;
                    case ItemType.Navigation:
                        if (currentNavigationDisplay != null)
                        {
                            Destroy(currentNavigationDisplay);
                            currentNavigationDisplay = null;
                            //basicNavigation.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }

                break;
            default:
                break;
        }
    }

    public override void OnAddItem(InventorySlot slot)
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
                            objectAttributes[j].value.AddModifier(slot.item.stats[i]);
                    }
                }

                if (slot.ItemObject.physicalDisplay != null)
                {
                    switch (slot.AllowedItems[0])
                    {
                        case ItemType.Hull:
                            if (currentHullDisplay != null)
                                Destroy(currentHullDisplay); 
                            currentHullDisplay = Instantiate(slot.ItemObject.physicalDisplay, transform);
                            basicHull.SetActive(false);
                            break;
                        case ItemType.Propeller:
                            if (currentPropellerDisplay != null)
                                Destroy(currentPropellerDisplay);
                            currentPropellerDisplay = Instantiate(slot.ItemObject.physicalDisplay, transform);
                            break;
                        case ItemType.Storage:
                            if (currentStorageDisplay != null)
                                Destroy(currentStorageDisplay);
                            currentStorageDisplay = Instantiate(slot.ItemObject.physicalDisplay, transform);
                            break;
                        case ItemType.Detection:
                            if (currentDetectionDisplay != null)
                                Destroy(currentDetectionDisplay);
                            currentDetectionDisplay = Instantiate(slot.ItemObject.physicalDisplay, transform);
                            break;
                        case ItemType.Navigation:
                            if (currentNavigationDisplay != null)
                                Destroy(currentNavigationDisplay);
                            currentNavigationDisplay = Instantiate(slot.ItemObject.physicalDisplay, transform);
                            break;
                        default:
                            break;
                    }
                }

                break;
            default:
                break;
        }
    }

    public override int GetModifiedStatValue(Stats statType)
    {
       return base.GetModifiedStatValue(statType);
    }

    public void AttributeModified(ShipAttributes attribute)
    {
        Debug.Log($"{attribute.type} changed to {attribute.value.ModifiedValue} points");
    }

    private void OnApplicationQuit()
    {
        inventory.inventoryContainer.Slots = new InventorySlot[24];
    }
}