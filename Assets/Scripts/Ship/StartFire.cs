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

    bool isInGracePeriod = false;
    bool isOnFire;
    public bool IsOnFire {  get { return isOnFire; } set {  isOnFire = value; } }

    private void Start()
    {
        fireVFX.SetActive(false);
        sparksVFX.SetActive(false);
       
        isOnFire = false;

        electricalDevice = FindAnyObjectByType<ElectricalDevice>();
        shipDamage = ShipDamage.Instance;
        

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

    private void Update()
    {
        if (shipDamage.GetModifiedStatValue(Stats.Health) >= shipDamage.GetPermanentModifiedStatValue(Stats.Health) - 20)
            sparksVFX.SetActive(true);
        else
            sparksVFX.SetActive(false);
    }

    private void ElectricalDevice_OnDegradation(object sender, EventArgs e)
    {
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
            FireActionStart();
            FireProbability = 0;
            FireTickDamage();
        }
    }


    IEnumerator FireTickDamage()
    {
        while (isOnFire)
        {
            Debug.Log("Taking Damage from fire");
            shipDamage.TakeDamage(shipDamage.BaseFireDamage);
            yield return new WaitForSeconds(fireTickInterval);
        }

        fireVFX.SetActive(false);
    }

    void FireActionStart()
    {
        if (isOnFire || isInGracePeriod) return;

        fireVFX.SetActive(true);
        isOnFire = true;
        StartCoroutine(FireTickDamage());
    }

    public void FireActionStop()
    {
        if (!isOnFire) return;

        fireVFX.SetActive(false);
        isOnFire = false;
        StartCoroutine(FireGracePeriod());
    }

    IEnumerator FireGracePeriod()
    {
        isInGracePeriod = true;
        yield return new WaitForSeconds(fireGracePeriodTime);
        isInGracePeriod = false;
    }

}
