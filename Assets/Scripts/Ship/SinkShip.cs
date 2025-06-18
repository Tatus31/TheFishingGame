using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkShip : MonoBehaviour
{
    public static Action<bool> OnShipSank;

    StableFloatingRigidBody stableFloatingRB;
    ChangeWaterLevelUnderDeck waterLevel;
    ShipDamage shipDamage;
    ShipMovement shipMovement;

    float sinkingBuoyancy = 0.98f;
    [SerializeField] float sinkingDelay = 3f;
    [SerializeField] float sinkTimer = 5f;
    float timer;
    bool isSinking = false;

    private void Awake()
    {
        stableFloatingRB = GetComponent<StableFloatingRigidBody>();
    }

    private void Start()
    {
        timer = sinkTimer;

        shipDamage = ShipDamage.Instance;
        waterLevel = ChangeWaterLevelUnderDeck.Instance;

        if (shipDamage != null)
            shipDamage.OnSinkingShipByDamage += ShipDamage_OnSinkingShipByDamage;

        if (waterLevel != null)
            waterLevel.OnSinkingShip += IncreaseWaterLevel_OnSinkingShip;
    }

    private void Update()
    {
        if (!isSinking)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            RespawnShip.Instance.RespawnShipManually();
            isSinking = false;
            timer = sinkTimer;
        }
    }

    private void ShipDamage_OnSinkingShipByDamage(object sender, EventArgs e)
    {
        StartCoroutine(StartSinking());
    }

    private void IncreaseWaterLevel_OnSinkingShip(object sender, bool e)
    {
        StartCoroutine(StartSinking());
    }

    IEnumerator StartSinking()
    {
        if (isSinking)
        {
            yield break; 
        }

        yield return new WaitForSeconds(sinkingDelay);

        if (stableFloatingRB != null)
        {
            stableFloatingRB.SafeFloating = false;
            stableFloatingRB.FloatToSleep = false;
            stableFloatingRB.Buoyancy = sinkingBuoyancy;
        }

        if (TryGetComponent<ShipMovement>(out shipMovement))
        {
            shipMovement.SetNeutralSpeed();
        }

        isSinking = true;
        OnShipSank?.Invoke(isSinking);
    }

    private void OnDestroy()
    {
        if (shipDamage != null)
        {
            shipDamage.OnSinkingShipByDamage -= ShipDamage_OnSinkingShipByDamage;
        }

        if (waterLevel != null)
        {
            waterLevel.OnSinkingShip -= IncreaseWaterLevel_OnSinkingShip;
        }
    }
}