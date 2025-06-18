using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkShip : MonoBehaviour
{
    public static Action<bool> OnShipSank;

    public StableFloatingRigidBody stableFloatingRB;
    ChangeWaterLevelUnderDeck waterLevel;
    ShipDamage shipDamage;

    float sinkingBuoyancy = 0.98f;
    public float buoyancy;
    [SerializeField] float sinkingDelay = 3f;
    [SerializeField] float sinkTimer = 5f;
    float timer;
    [SerializeField] bool isSinking = false;

    public float Buoyancy { get { return buoyancy; } set { buoyancy = value; } }

    private void Awake()
    {
        stableFloatingRB = GetComponent<StableFloatingRigidBody>();
    }

    private void Start()
    {
        timer = sinkTimer;

        shipDamage = ShipDamage.Instance;
        waterLevel = ChangeWaterLevelUnderDeck.Instance;

        buoyancy = stableFloatingRB.Buoyancy;

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

            stableFloatingRB.SafeFloating = true;
            stableFloatingRB.FloatToSleep = true;
            stableFloatingRB.Buoyancy = buoyancy;

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

        ShipMovement shipMovement = GetComponent<ShipMovement>();
        shipMovement.SetNeutralSpeed();

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