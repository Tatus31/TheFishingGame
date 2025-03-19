using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShipToLocation : ShipTransporter
{
    public static MoveShipToLocation Instance;

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;
    }
}
