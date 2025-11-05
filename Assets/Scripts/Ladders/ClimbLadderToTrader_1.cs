using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLadderToTrader_1 : ShipTransporter
{
    public static ClimbLadderToTrader_1 Instance;

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
