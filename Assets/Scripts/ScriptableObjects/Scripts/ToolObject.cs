using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/Tools/Fire Extinguisher")]
public class ToolObject : ItemObject
{
    private void Awake()
    {
        type = ItemType.fireExtinguisher;
    }
}
