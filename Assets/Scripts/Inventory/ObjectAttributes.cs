using System;
using UnityEngine;

[Serializable]
public class ObjectAttributes
{
    [NonSerialized]
    public ObjectInventory parent;
    public Stats type;

    [SerializeField]
    int savedBaseValue;
    [SerializeField]
    int savedModifiedValue;

    ModifiableInt value;
    public ModifiableInt Value
    {
        get
        {
            if (value == null)
            {
                value = new ModifiableInt(AttributeModified);
                value.SetValues(savedBaseValue, savedModifiedValue);
            }
            return value;
        }
    }

    public void SetParent(ObjectInventory parent)
    {
        this.parent = parent;
        value = new ModifiableInt(AttributeModified);
        value.SetValues(savedBaseValue, savedModifiedValue);
    }

    public void AttributeModified()
    {
        savedBaseValue = Value.BaseValue;
        savedModifiedValue = Value.ModifiedValue;

        parent?.AttributeModified(this);
    }

    public void SetPermamentAttributeModified() => Value.SetPermamentAttributeModified();

    public int GetSavedModifiedValue() => savedModifiedValue;
    public int GetSavedBaseValue() => savedBaseValue;
    public int GetPermanentBaseValue() => Value.PermanentBaseValue;
    public int GetPermanentModifiedValue() => Value.PermanentModifiedValue;
}