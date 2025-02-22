using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();

[Serializable]
public class ModifiableInt
{
    private int baseValue;
    public int BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; UpdateModifiedValue(); }
    }

    private int modifiedValue;
    public int ModifiedValue
    {
        get { return modifiedValue; }
        private set { modifiedValue = value; }
    }

    // List of modifiers should not be serialized since they're recreated during runtime
    [NonSerialized]
    private List<IModifier> modifiers = new List<IModifier>();

    public event ModifiedEvent ValueModified;

    public ModifiableInt(ModifiedEvent modifiedEvent = null)
    {
        modifiers = new List<IModifier>();
        if (modifiedEvent != null)
        {
            ValueModified += modifiedEvent;
        }
    }

    // Method to set both values without triggering updates
    public void SetValues(int baseVal, int modifiedVal)
    {
        this.baseValue = baseVal;
        this.modifiedValue = modifiedVal;
    }

    public void UpdateModifiedValue()
    {
        if (modifiers == null)
            modifiers = new List<IModifier>();

        var valueToAdd = 0;
        foreach (var modifier in modifiers)
        {
            modifier.AddValue(ref valueToAdd);
        }

        modifiedValue = baseValue + valueToAdd;
        ValueModified?.Invoke();
    }

    public void AddModifier(IModifier modifier)
    {
        if (modifiers == null)
            modifiers = new List<IModifier>();

        if (!modifiers.Contains(modifier))
        {
            modifiers.Add(modifier);
            UpdateModifiedValue();
        }
    }

    public void RemoveModifier(IModifier modifier)
    {
        if (modifiers == null)
            modifiers = new List<IModifier>();

        if (modifiers.Contains(modifier))
        {
            modifiers.Remove(modifier);
            UpdateModifiedValue();
        }
    }

    public void SetModifiedValueDirectly(int newValue)
    {
        modifiedValue = newValue;
        baseValue = newValue; 
        ValueModified?.Invoke();
    }
}
