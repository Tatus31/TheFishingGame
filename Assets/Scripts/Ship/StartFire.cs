using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;



public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] private float fireTickInterval = 1.0f;
    [SerializeField] private float FireProbability;
    [SerializeField] private float fireGracePeriodTime = 10f;

    private float FireProbabilityMaxValue = 500f;

    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;
    Coroutine fireTickCoroutine;

    bool isInGracePeriod = false;
    public bool isOnFire;
    public bool IsOnFire {  get { return isOnFire; } set {  isOnFire = value; } }

    private void Start()
    {
        fireVFX.SetActive(false);
        sparksVFX.SetActive(false);
       
        isOnFire = false;
        shipDamage = ShipDamage.Instance;

        electricalDevice = FindAnyObjectByType<ElectricalDevice>();

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
        ChangeWaterLevelUnderDeck.Instance.OnShipCatchingWater += ChangeWaterLevelUnderDeck_OnShipCatchingWater;
        StartFireWhenInCloud.OnShipInCloud += OnShipInCloudFire;
    }

    private void ChangeWaterLevelUnderDeck_OnShipCatchingWater(object sender, bool e)
    {
#if UNITY_EDITOR
        Debug.Log("there is water underdeck");
#endif
        FireActionStop();
    }

    private void ElectricalDevice_OnDegradation(object sender, EventArgs e)
    {
        if(electricalDevice.CurrentDegradation >= ElectricalDevice.DegradationCondition.Average)
        {
            sparksVFX.SetActive(true);
        }
        else
        {
            sparksVFX.SetActive(false);
        }

        if (electricalDevice.CurrentDegradation == ElectricalDevice.DegradationCondition.Bad)
        {
            FireActionStart();
        }
    }

    private void OnShipInCloudFire()
    {       
        FireProbability++;

        if (FireProbability >= FireProbabilityMaxValue)
        {
            FireProbability = 0;
            FireActionStart();
        }
    }


    IEnumerator FireTickDamage()
    {
        while (isOnFire)
        {
            Debug.Log($"isOnFire inside {isOnFire}");

            if (!isOnFire)
            {
                Debug.Log("Fire stopped");
                yield break;
            }

            shipDamage.TakeDamage(shipDamage.BaseFireDamage);

            yield return new WaitForSeconds(fireTickInterval);
        }

        fireTickCoroutine = null;
    }

    void FireActionStart()
    {
        if (isOnFire || isInGracePeriod) return;

        if (fireTickCoroutine != null)
        {
            StopCoroutine(fireTickCoroutine);
            fireTickCoroutine = null;
        }

        fireVFX.SetActive(true);
        isOnFire = true;
        fireTickCoroutine = StartCoroutine(FireTickDamage());
    }

    public void FireActionStop()
    {
        if (!isOnFire) return;

        isOnFire = false;

        if (fireTickCoroutine != null)
        {
            StopCoroutine(fireTickCoroutine);
            fireTickCoroutine = null;
        }

        fireVFX.SetActive(false);
        StartCoroutine(FireGracePeriod());
    }

    IEnumerator FireGracePeriod()
    {
        isInGracePeriod = true;
        yield return new WaitForSeconds(fireGracePeriodTime);
        isInGracePeriod = false;
    }

}
