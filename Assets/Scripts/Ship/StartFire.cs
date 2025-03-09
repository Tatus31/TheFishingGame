using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] float fireTickInterval = 1.0f;

    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;
    StartFireWhenInCloud StartFireWhenInCloud;

    bool isSparking;

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
        StartFireWhenInCloud.OnShipInCloud += OnShipInCloudFire;
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
        Debug.Log("Statek jest w chmurze");
        
        if (Random.Range(-10,10) == 5)
        {   
            Debug.Log("Styrta sie Poli!");
            FireActionStart(); 
        }

         
    }


    IEnumerator FireTickDamage()
    {
        while (isOnFire)
        {
            shipDamage.TakeTickDamage(shipDamage.BaseFireDamage);
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
}
