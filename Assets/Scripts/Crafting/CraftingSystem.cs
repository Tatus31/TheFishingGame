using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ArtifactRewardPair
{
    public ItemObject artifactItem;   
    public ItemObject rewardItem;    
    public int requiredSacrificeValue; 
}

public class CraftingSystem : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject craftingInventory;

    [SerializeField] Slider sacrificeSlider;
    [SerializeField] int totalSacrificeValue = 0;
    [SerializeField] List<ArtifactRewardPair> artifactRewards = new List<ArtifactRewardPair>();

    ItemObject currentArtifact = null;
    ItemObject currentReward = null;

    InventorySlot sacrificeSlot = null;

    int currentRequiredValue = 0;
    bool isProcessingReward = false;

    public int TotalSacrificeValue { get { return totalSacrificeValue; } }

    private void Start()
    {
        for (int i = 0; i < craftingInventory.GetSlots.Length; i++)
        {
            craftingInventory.GetSlots[i].OnAfterUpdate += OnAddItem;
        }
    }

    public void OnAddItem(InventorySlot slot)
    {
        if (isProcessingReward)
        {
            return;
        }

        if (slot.ItemObject == null)
            return;

        switch (slot.parent.inventory.type)
        {
            case InterfaceType.Crafting:
                if (slot.ItemObject.type == ItemType.Artifact)
                {
                    if (currentArtifact != null)
                    {
                        Debug.LogWarning("Cannot sacrifice multiple artifacts! Complete the current ritual first.");

                        if (inventory != null)
                        {
                            inventory.AddItem(slot.item, 1, slot.item.weight);
                        }

                        isProcessingReward = true;
                        slot.UpdateSlot(new Item(), 0);
                        isProcessingReward = false;
                        return;
                    }

                    ArtifactRewardPair pair = FindArtifactRewardPair(slot.ItemObject);
                    if (pair != null)
                    {
                        currentArtifact = slot.ItemObject;
                        currentReward = pair.rewardItem;
                        currentRequiredValue = pair.requiredSacrificeValue;
                        sacrificeSlot = slot; 

                        Debug.Log($"Artifact {currentArtifact.name} placed. Need {currentRequiredValue} sacrifice value to complete ritual.");

                        isProcessingReward = true;
                        slot.UpdateSlot(new Item(), 0);
                        isProcessingReward = false;
                    }
                    else
                    {
                        Debug.LogWarning($"No reward configured for artifact: {slot.ItemObject.name}");

                        if (inventory != null)
                        {
                            inventory.AddItem(slot.item, 1, slot.item.weight);
                        }

                        isProcessingReward = true;
                        slot.UpdateSlot(new Item(), 0);
                        isProcessingReward = false;
                    }
                }
                else if (slot.item.id > -1 && !slot.item.isQuestItem)
                {
                    if (currentArtifact == null)
                    {
                        Debug.LogWarning("Please place an artifact first before sacrificing items.");

                        if (inventory != null)
                        {
                            inventory.AddItem(slot.item, 1, slot.item.weight);
                        }

                        isProcessingReward = true;
                        slot.UpdateSlot(new Item(), 0);
                        isProcessingReward = false;
                        return;
                    }

                    int valueToAdd = slot.item.sacrificeValue;
                    if(slot.amount > 1)
                        valueToAdd = slot.amount * slot.item.sacrificeValue; 

                    totalSacrificeValue += valueToAdd;

                    if(sacrificeSlider != null)
                        sacrificeSlider.value = (float)totalSacrificeValue / currentRequiredValue;

                    Debug.Log($"Sacrificed {slot.item.name} for {valueToAdd} points. Total: {totalSacrificeValue}/{currentRequiredValue}");

                    sacrificeSlot = slot;

                    isProcessingReward = true;
                    slot.UpdateSlot(new Item(), 0);
                    isProcessingReward = false;

                    if (totalSacrificeValue >= currentRequiredValue)
                    {
                        Debug.Log("Ritual successful!");
                        CreateRewardItem();
                    }
                }
                break;

            default:
                break;
        }
    }

    private ArtifactRewardPair FindArtifactRewardPair(ItemObject artifact)
    {
        foreach (var pair in artifactRewards)
        {
            if (pair.artifactItem == artifact)
            {
                return pair;
            }
        }
        return null;
    }

    private void CreateRewardItem()
    {
        if (currentReward != null && sacrificeSlot != null)
        {
            Item newItem = new Item(currentReward);

            Debug.Log($"Creating {newItem.name} from the ritual with {currentArtifact.name}!");

            isProcessingReward = true;
            sacrificeSlot.UpdateSlot(newItem, 1);
            isProcessingReward = false;

            ResetRitual();
        }
        else
        {
            Debug.LogError("No reward item configured or no valid sacrifice slot!");
            ResetRitual();
        }
    }

    public void ResetRitual()
    {
        totalSacrificeValue = 0;
        currentArtifact = null;
        currentReward = null;
        currentRequiredValue = 0;
        sacrificeSlot = null;
    }

    public void CancelRitual()
    {
        if (currentArtifact != null)
        {
            Debug.Log("Ritual canceled.");
            ResetRitual();
        }
    }
}