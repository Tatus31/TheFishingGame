using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;



public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] float fireTickInterval = 1.0f;
    [SerializeField] float FireProbability;
    [SerializeField] float fireGracePeriodTime = 10f;

    float FireProbabilityMaxValue = 500f;
    float fireTime = 0f;

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
            fireTime += Time.deltaTime;

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

        fireTime = (float)Math.Round(fireTime, 2);

        var analyticsData = new Dictionary<string, object>
        {
            { "timeShipWasOnFire", fireTime }
        };

        AnalyticsEvents.SendAnalyticsEvent("OnShipOnFireTime", analyticsData);
        fireTime = 0f;
    }

    IEnumerator FireGracePeriod()
    {
        isInGracePeriod = true;
        yield return new WaitForSeconds(fireGracePeriodTime);
        isInGracePeriod = false;
    }

}
