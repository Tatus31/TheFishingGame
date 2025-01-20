using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wood", menuName = "Inventory/Items/Wood")]
public class WoodItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.Wood;
    }
}
