using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerAttributes
{
    [NonSerialized]
    public Player parent;
    public Stats type;
    public ModifiableInt value;

    public void SetParent(Player parent)
    {
        this.parent = parent;
        value = new ModifiableInt(AttributeModified);
    }

    public void AttributeModified()
    {
        parent.AttributeModified(this);
    }
}
