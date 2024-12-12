using UnityEngine;
using static Flee;

public struct FleeData
{
    public Vector3 Position;
    public FleeDirection Direction;

    public FleeData(Vector3 position, FleeDirection direction)
    {
        Position = position;
        Direction = direction;
    }
}

