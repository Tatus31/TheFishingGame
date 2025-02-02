using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Items/Database")]
public class ItemDatabaseObject : ScriptableObject,ISerializationCallbackReceiver
{
    public ItemObject[] ItemObjects;
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

    public void OnAfterDeserialize()
    {
        GetItem  = new Dictionary<int, ItemObject>();
        for (int i = 0; i < ItemObjects.Length; i++)
        {
            GetItem .Add(i, ItemObjects[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        GetItem = new Dictionary<int, ItemObject>();

        for (int i = 0; i < ItemObjects.Length; i++)
        {
            ItemObjects[i].data.id = i;
            GetItem[i] = ItemObjects[i];
        }
    }
}
