using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Metal", menuName = "Inventory/Items/Metal")]
public class MetalItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.Metal;
    }
}
