using System;

public class StatModifiedEventArgs : EventArgs
{
    public Stats StatType { get; }
    public int ModifiedValue { get; }

    public StatModifiedEventArgs(Stats statType, int modifiedValue)
    {
        StatType = statType;
        ModifiedValue = modifiedValue;
    }
}