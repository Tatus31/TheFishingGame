using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Inventory/Items/Fish")]
public class FishItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.Fish;
    }
}
