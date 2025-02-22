using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();

[Serializable]
public class ModifiableInt
{

    int permanentBaseValue;
    public int PermanentBaseValue { get { return permanentBaseValue; } set {  permanentBaseValue = value; } }

    int permanentModifiedValue;
    public int PermanentModifiedValue { get { return permanentModifiedValue; } set { permanentModifiedValue = value; } }

    int baseValue;
    public int BaseValue
    {
        get { return baseValue; }
        set { baseValue = value; UpdateModifiedValue(); }
    }

    int modifiedValue;
    public int ModifiedValue
    {
        get { return modifiedValue; }
        private set { modifiedValue = value; }
    }

    [NonSerialized]
    List<IModifier> modifiers = new List<IModifier>();

    public event ModifiedEvent ValueModified;

    public ModifiableInt(ModifiedEvent modifiedEvent = null)
    {
        modifiers = new List<IModifier>();
        if (modifiedEvent != null)
        {
            ValueModified += modifiedEvent;
        }
    }

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

    public void SetPermamentAttributeModified()
    {
        permanentBaseValue = baseValue;
        permanentModifiedValue = modifiedValue;
    }
}
