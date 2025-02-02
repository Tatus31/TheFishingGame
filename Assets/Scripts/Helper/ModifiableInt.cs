using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ModifiedEvent();

[Serializable]
public class ModifiableInt 
{
    [SerializeField]
    int baseValue;
    public int BaseValue { get { return baseValue; } set {  baseValue = value; UpdateModifiedValue(); } }
    [SerializeField]
    int modifiedValue;
    public int ModifiedValue { get { return modifiedValue; } set {  modifiedValue = value; } }

    public List<IModifier> modifiers = new List<IModifier>();

    public event ModifiedEvent ValueModified;
    public ModifiableInt(ModifiedEvent modifiedEvent = null)
    {
        modifiedValue = baseValue;
        if (modifiedEvent != null)
        {
            ValueModified += modifiedEvent;
        }
    }

    public void RegisterModEvent(ModifiedEvent modifiedEvent)
    {
        ValueModified += modifiedEvent;
    }

    public void UnregisterModEvent(ModifiedEvent modifiedEvent)
    {
        ValueModified += modifiedEvent;
    }

    public void UpdateModifiedValue()
    {
        var valueToAdd = 0;
        for (int i = 0; i < modifiers.Count; i++)
        {
            modifiers[i].AddValue(ref valueToAdd);
        }
        modifiedValue = baseValue + valueToAdd;
        if(ValueModified != null)
        {
            ValueModified.Invoke();
        }
    }

    public void AddModifier(IModifier modifier)
    {
        modifiers.Add(modifier);
        UpdateModifiedValue();
    }

    public void RemoveModifier(IModifier modifier)
    {
        modifiers.Remove(modifier);
        UpdateModifiedValue();
    }
}
