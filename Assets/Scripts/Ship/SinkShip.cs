using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkShip : MonoBehaviour
{
    Rigidbody shipRB;
    StableFloatingRigidBody stableFloatingRB;
    IncreaseWaterLevel waterLevel;

    float sinkingBuoyancy = 0.98f;
    [SerializeField] float sinkingDelay = 3f;

    private void Awake()
    {
        shipRB = GetComponent<Rigidbody>();
        stableFloatingRB = GetComponent<StableFloatingRigidBody>();
    }

    private void Start()
    {
        waterLevel = IncreaseWaterLevel.Instance;
        waterLevel.OnSinkingShip += IncreaseWaterLevel_OnSinkingShip;
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
    }
}