using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Fish,
    Artifact,
    Wood
}

public abstract class ItemObject : ScriptableObject
{
    [Header("Item Settings")]
    public int id;
    public Sprite uiDisplay;
    public ItemType type;
    [TextArea(15,20)] public string description;
}

[Serializable]
public class Item
{
    public string name;
    public int id;
    public Item(ItemObject itemObject)
    {
        name = itemObject.name;
        id = itemObject.id;
    }
}