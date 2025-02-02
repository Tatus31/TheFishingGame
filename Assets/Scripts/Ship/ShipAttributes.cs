using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipAttributes
{
    [NonSerialized]
    public Ship parent;
    public ShipStats type;
    public ModifiableInt value;

    public void SetParent(Ship parent)
    {
        this.parent = parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
