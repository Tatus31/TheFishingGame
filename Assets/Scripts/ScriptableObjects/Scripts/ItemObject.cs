using System;
using UnityEditor.TerrainTools;
using UnityEngine;

public enum ItemType
{
    Fish,
    Artifact,
    Wood,
    Hull,
    Propeller,
    Storage,
    Detection,
    fireExtinguisher,
    Metal
}

public enum Stats
{
    Speed,
    Health,
    DetectionRange,
    StorageSpace,
    Capacity
}

public abstract class ItemObject : ScriptableObject
{
    [Header("Item Settings")]
    public Sprite uiDisplay;
    public GameObject physicalDisplay;
    public bool stackable;
    public ItemType type;
    [TextArea(15,20)] public string description;
    public Item data = new Item();
}

[Serializable]
public class Item
{
    public string name;
    public int id = -1;
    public int weight;
    public string description;
    public int sacrificeValue;
    public bool isQuestItem;
    public StatModification[] stats;

    public Item()
    {
        name = "";
        id = -1;
        description = "";
        weight = 1;
        sacrificeValue = 1;
        isQuestItem = false;
    }

    public Item(ItemObject itemObject)
    {
        name = itemObject.name;
        id = itemObject.data.id;
        weight = itemObject.data.weight;
        stats = itemObject.data.stats;
        sacrificeValue = itemObject.data.sacrificeValue;
        isQuestItem = itemObject.data.isQuestItem;
    }
}

[Serializable]
public class StatModification : IModifier
{
    public Stats stats;
    public int value;

    public void AddValue(ref int baseValue)
    {
        baseValue += value;
    }
}