using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnShip : ShipTransporter
{
    public static RespawnShip Instance;

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

    private void Start()
    {
        SinkShip.OnShipSank += SinkShip_OnShipSank;
    }

    private void SinkShip_OnShipSank(bool obj)
    {
        Debug.Log("reset");

        StartFire startFire = FindObjectOfType<StartFire>();
        ShipDamage shipDamage = FindObjectOfType<ShipDamage>();
        ShipRepairPoints shipRepairPoints = FindObjectOfType<ShipRepairPoints>();
        ElectricalDevice electricalDevice = FindObjectOfType<ElectricalDevice>();

        startFire.IsOnFire = false;
        shipDamage.RestoreHealth(shipDamage.GetModifiedStatValue(Stats.Health));
        shipRepairPoints.ResetAllRepairPoints();
        electricalDevice.RepairDevice(100);
    }
}
