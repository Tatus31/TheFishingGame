using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] private float fireTickInterval = 1.0f;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float FireProbability;
    [SerializeField] private float FireProbabilityMaxValue;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private Transform shipTransform;
    
    
    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;
    StartFireWhenInCloud startfirewhenincloud;

    bool isSparking;
    bool isUsed;
    bool isOnFire;
    bool isWaterUnderDeck;
    public bool IsOnFire {  get { return isOnFire; } set {  isOnFire = value; } }

    private void Start()
    {
        fireVFX.SetActive(false);
        sparksVFX.SetActive(false);

        isOnFire = false;

        electricalDevice = GetComponent<ElectricalDevice>();
        startfirewhenincloud = (StartFireWhenInCloud)FindAnyObjectByType(typeof(StartFireWhenInCloud));
        shipDamage = ShipDamage.Instance;

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
        ChangeWaterLevelUnderDeck.Instance.OnShipCatchingWater += ChangeWaterLevelUnderDeck_OnShipCatchingWater;
        startfirewhenincloud.OnShipInCloud += OnShipInCloudFire;
    }

    private void ChangeWaterLevelUnderDeck_OnShipCatchingWater(object sender, bool e)
    {
        Debug.Log("there is water underdeck");
        FireActionStop();
    }

    private void Update()
    {
        if (shipDamage.GetModifiedStatValue(Stats.Health) == shipDamage.GetPermanentModifiedStatValue(Stats.Health))
            isSparking = false;
        else
            isSparking = true;

        if(isSparking)
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
            RandomPosFireStart();
        }
        Debug.Log(FireProbability);

    }


    IEnumerator FireTickDamage()
    {
        while (isOnFire)
        {
            shipDamage.TakeDamage(shipDamage.BaseFireDamage);
            yield return new WaitForSeconds(fireTickInterval);
        }

        fireVFX.SetActive(false);
    }

    void FireActionStart()
    {
        Debug.Log("start fire");
        fireVFX.SetActive(true);
        isOnFire = true;
        StartCoroutine(FireTickDamage());
    }

    void FireActionStop()
    {
        fireVFX.SetActive(false);
        isOnFire = false;
    }

    void RandomPosFireStart()
    {
        Vector3 randomPoint = Random.insideUnitCircle * radius;
        Vector3 _randomFirePos = new Vector3(randomPoint.x, shipTransform.position.y, randomPoint.y) + shipTransform.position;
        for (int i = 0; i <= 2; i++) 
        {
            
                Instantiate(fireVFX, _randomFirePos, quaternion.identity);    
            
            

        }

       
    }
}
