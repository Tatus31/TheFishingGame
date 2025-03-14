using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;


public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] private float fireTickInterval = 1.0f;
    [SerializeField] List<StartFire> FirePointsList = new List<StartFire>();
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    private int damageValue = default;
    [SerializeField] private int FireProbability;
    
    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;
    StartFireWhenInCloud StartFireWhenInCloud;

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
        StartFireWhenInCloud StartFireWhenInCloud = FindObjectOfType<StartFireWhenInCloud>();
        shipDamage = ShipDamage.Instance;

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
        ChangeWaterLevelUnderDeck.Instance.OnShipCatchingWater += ChangeWaterLevelUnderDeck_OnShipCatchingWater;

        if(StartFireWhenInCloud != null)
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
        if (FireProbability >= 1000)
        {
            FireActionStart();
            PosFireStart(30);
        }
#if UNITY_EDITOR
        Debug.Log(FireProbability);
#endif
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
#if UNITY_EDITOR
        Debug.Log("start fire");
#endif
        fireVFX.SetActive(true);
        isOnFire = true;
        StartCoroutine(FireTickDamage());
    }

    void FireActionStop()
    {
        fireVFX.SetActive(false);
        isOnFire = false;
    }

    void PosFireStart(int damagePerFirepoint)
    {
        StartFire unusedPoint = FirePointsList.Find(point => !point.isUsed);
        if (unusedPoint != null)
        {
            Vector3 worldPoint = transform.TransformPoint(unusedPoint.position);
            Quaternion worldRotation = transform.rotation * Quaternion.Euler(unusedPoint.rotation);
            Instantiate(fireVFX, worldPoint, worldRotation, transform);
            unusedPoint.damageValue = damagePerFirepoint;
#if UNITY_EDITOR
            Debug.Log($"Zadaje {unusedPoint.damageValue}");
#endif
            unusedPoint.isUsed = true;
           
        }
    }
}
