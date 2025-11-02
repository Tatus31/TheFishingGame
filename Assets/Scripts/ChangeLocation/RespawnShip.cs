using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnShip : ShipTransporter
{
    public static RespawnShip Instance;

    private StartFire _startFire;
    private ShipDamage _shipDamage;
    private ShipRepairPoints _shipRepairPoints;
    private ElectricalDevice _electricalDevice;
    private ShipMovement _shipMovement;
    private StickToShip _stickToShip;
    
    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
            Destroy(Instance);
            
        }

        Instance = this;
    }

    private void Start()
    {
        _startFire = FindObjectOfType<StartFire>();
        _shipDamage = FindObjectOfType<ShipDamage>();
        _shipRepairPoints = FindObjectOfType<ShipRepairPoints>();
        _electricalDevice = FindObjectOfType<ElectricalDevice>();
        _shipMovement = FindObjectOfType<ShipMovement>();
        _stickToShip = FindObjectOfType<StickToShip>();
        
        SinkShip.OnShipSank += SinkShip_OnShipSank;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SinkShip_OnShipSank(bool isSinking)
    {
        if (isSinking)
        {
            _stickToShip.IsControllingShip = false;
            _shipMovement.SetNeutralSpeed();
            _shipMovement.HaltShip();
            _startFire.FireActionStop();
            _shipDamage.RestoreHealth(_shipDamage.GetPermanentModifiedStatValue(Stats.Health));
            _electricalDevice.RepairDevice(100);
        }
    }
}
