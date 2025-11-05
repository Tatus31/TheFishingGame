using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLadderToTrader_2 : ShipTransporter
{
    public static ClimbLadderToTrader_2 Instance;

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
