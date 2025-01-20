using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Inventory/Items/Artifact")]
public class ArtifactItem : ItemObject
{
    private void Awake()
    {
        type = ItemType.Artifact;
    }
}
