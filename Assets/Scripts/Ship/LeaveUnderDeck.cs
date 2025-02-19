using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveUnderDeck : ShipTransporter
{
    public static LeaveUnderDeck Instance;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;
    }
}
