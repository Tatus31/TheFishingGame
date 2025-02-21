using System;
using UnityEngine;

[Serializable]
public class ObjectAttributes
{
    [NonSerialized]
    public ObjectInventory parent;
    public Stats type;

    // Serialize the ModifiableInt as separate fields
    [SerializeField]
    private int savedBaseValue;
    [SerializeField]
    private int savedModifiedValue;

    // The actual ModifiableInt instance
    private ModifiableInt _value;
    public ModifiableInt value
    {
        get
        {
            if (_value == null)
            {
                _value = new ModifiableInt(AttributeModified);
                // Restore saved values
                _value.SetValues(savedBaseValue, savedModifiedValue);
            }
            return _value;
        }
    }

    public void SetParent(ObjectInventory parent)
    {
        this.parent = parent;
        // Initialize with saved values
        _value = new ModifiableInt(AttributeModified);
        _value.SetValues(savedBaseValue, savedModifiedValue);
    }

    public void AttributeModified()
    {
        // Save current values when modified
        savedBaseValue = value.BaseValue;
        savedModifiedValue = value.ModifiedValue;
        parent?.AttributeModified(this);
    }
}