using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkShip : MonoBehaviour
{
    Rigidbody shipRB;
    StableFloatingRigidBody stableFloatingRB;
    ChangeWaterLevelUnderDeck waterLevel;
    ShipDamage shipDamage;
    ShipMovement shipMovement;

    float sinkingBuoyancy = 0.98f;
    [SerializeField] float sinkingDelay = 3f;

    private void Awake()
    {
        shipRB = GetComponent<Rigidbody>();
        stableFloatingRB = GetComponent<StableFloatingRigidBody>();
    }

    private void Start()
    {
        if (shipDamage == null)
            return;

        if (waterLevel == null)
        {
            return;
        }

        shipDamage = ShipDamage.Instance;
        waterLevel = ChangeWaterLevelUnderDeck.Instance;

        waterLevel.OnSinkingShip += IncreaseWaterLevel_OnSinkingShip;
        shipDamage.OnSinkingShipByDamage += ShipDamage_OnSinkingShipByDamage;
    }

    private void ShipDamage_OnSinkingShipByDamage(object sender, EventArgs e)
    {
        StartCoroutine(StartSinking());
    }

    private void IncreaseWaterLevel_OnSinkingShip(object sender, bool e)
    {
        StartCoroutine(StartSinking());
    }

    private IEnumerator StartSinking()
    {
        yield return new WaitForSeconds(sinkingDelay);

        stableFloatingRB.SafeFloating = false;
        stableFloatingRB.FloatToSleep = false;
        stableFloatingRB.Buoyancy = sinkingBuoyancy;

        if (TryGetComponent<ShipMovement>(out shipMovement))
        {
            shipMovement.SetNeutralSpeed();
        }

    }
}