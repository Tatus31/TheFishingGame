using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectAttributes
{
    [NonSerialized]
    public ObjectInventory parent;
    public Stats type;
    public ModifiableInt value;

    public void SetParent(ObjectInventory parent)
    {
        this.parent = parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
