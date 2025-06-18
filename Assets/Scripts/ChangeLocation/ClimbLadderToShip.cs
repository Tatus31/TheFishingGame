using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ClimbLadderToShip : ShipTransporter
{
    public static ClimbLadderToShip Instance;

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
