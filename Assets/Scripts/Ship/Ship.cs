using PSX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : ObjectInventory
{
    public static event EventHandler<ItemType> OnEquipmentChange;
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

    FogController fogController;

    protected override void Start()
    {
        base.Start();

        fogController = GetComponent<FogController>();
        UpdateFogEnd();

        foreach (var attribute in objectAttributes)
        {
            attribute.SetPermamentAttributeModified();
        }

        ElectricalDevice.OnDegradation += HandleDeviceDegradation;
    }

    private void OnDestroy()
    {
        ElectricalDevice.OnDegradation -= HandleDeviceDegradation;
    }

    private void HandleDeviceDegradation(object sender, System.EventArgs e)
    {
        if (sender is ElectricalDevice device && device.DeviceStats.deviceStats == Stats.DetectionRange)
        {
            UpdateFogEnd();
        }
    }

    public int GetDegradationModifier(ElectricalDevice.DegradationCondition condition, int statDegradation)
    {
        return condition switch
        {
            ElectricalDevice.DegradationCondition.Perfect => statDegradation,
            ElectricalDevice.DegradationCondition.Ok => -statDegradation,
            ElectricalDevice.DegradationCondition.Average => -statDegradation * 2,
            ElectricalDevice.DegradationCondition.Bad => -statDegradation * 3,
            _ => 0
        };
    }

    public override void OnRemoveItem(InventorySlot slot)
    {
        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Inventory:
                break;
            case InterfaceType.Equipment:
                bool isDetectionItem = slot.AllowedItems[0] == ItemType.Detection;

                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < objectAttributes.Length; j++)
                    {
                        if (objectAttributes[j].type == slot.item.stats[i].stats)
                            objectAttributes[j].Value.RemoveModifier(slot.item.stats[i]);

                    }
                }

                if (isDetectionItem)
                {
                    UpdateFogEnd();
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

                ItemType itemType = slot.AllowedItems[0];

                for (int i = 0; i < slot.item.stats.Length; i++)
                {
                    for (int j = 0; j < objectAttributes.Length; j++)
                    {
                        if (objectAttributes[j].type == slot.item.stats[i].stats)
                            objectAttributes[j].Value.AddModifier(slot.item.stats[i]);
                    }
                }

                switch (itemType)
                {
                    case ItemType.Detection:
                        UpdateFogEnd();
                        OnEquipmentChange?.Invoke(this, itemType);
                        break;

                    case ItemType.Hull:
                    case ItemType.Propeller:
                    case ItemType.Storage:
                        OnEquipmentChange?.Invoke(this, itemType);
                        break;
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
                        default:
                            break;
                    }
                }

                break;
            default:
                break;
        }
    }

    public void UpdateFogEnd()
    {
        if (fogController != null)
        {
            int detectionRange = GetModifiedStatValue(Stats.DetectionRange);
            fogController.SetFogEnd(detectionRange);
            Debug.Log($"changed fog to {detectionRange + fogController.defaultFogEnd}");
        }
    }

    public override int GetModifiedStatValue(Stats statType)
    {
        int baseValue = base.GetModifiedStatValue(statType);

        if (statType == Stats.DetectionRange)
        {
            ElectricalDevice[] detectionDevices = FindObjectsOfType<ElectricalDevice>();
            foreach (var device in detectionDevices)
            {
                if (device.DeviceStats.deviceStats == Stats.DetectionRange)
                {
                    baseValue += GetDegradationModifier(device.CurrentDegradation, device.DeviceStats.statDegradation);
                    Debug.Log($"current deg modifier {baseValue}");
                }
            }
        }

        return Mathf.Max(0, baseValue);
    }

    public int GetSavedModifiedStatValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.GetSavedModifiedValue();
            }
        }

        return 0;
    }

    public int GetSavedBaseStatValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.GetSavedBaseValue();
            }
        }
        return 0;
    }

    public int GetPermanentSavedModifiedStatValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.GetPermanentModifiedValue();
            }
        }
        return 0;
    }

    public int GetPermanentSavedBaseStatValue(Stats statType)
    {
        foreach (var attribute in objectAttributes)
        {
            if (attribute.type == statType)
            {
                return attribute.GetPermanentBaseValue();
            }
        }
        return 0;
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