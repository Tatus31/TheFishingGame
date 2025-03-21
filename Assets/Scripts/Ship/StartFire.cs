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
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private float FireProbability;
    private float FireProbabilityMaxValue = 500f;
    //private Random random = new Random();
    [SerializeField] private List<GameObject> FirePointList = new List<GameObject>();
   
    
    
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
        if(startfirewhenincloud != null)
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
            FireProbability = 0; // Resetujemy licznik po nowym ogniu
        }

        Debug.Log($"🔥 Aktualne ryzyko pożaru: {FireProbability}");

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
        if (isOnFire) return;
        
        Debug.Log("start fire");
        fireVFX.SetActive(true);
        isOnFire = true;
        RandomPosFireStart();
        //StartCoroutine(FireTickDamage());
    }

    void FireActionStop()
    {
        fireVFX.SetActive(false);
        isOnFire = false;
        
        foreach (GameObject obj in FirePointList)
        {
            obj.SetActive(false);
        }
    }

    void RandomPosFireStart()
    {
        if (FirePointList.Count == 0) return;  // Sprawdzenie, czy lista nie jest pusta

        // Filtrowanie listy: znajdź punkty, które jeszcze się nie palą
        List<GameObject> wolnePunkty = FirePointList.FindAll(p => !p.activeSelf);

        if (wolnePunkty.Count == 0)
        {
            Debug.Log("Wszystkie punkty ognia są już aktywne!");
            return; // Jeśli nie ma dostępnych miejsc, kończymy
        }

        // Wybieramy losowy punkt spośród dostępnych
        int losowyIndeks = UnityEngine.Random.Range(0, wolnePunkty.Count);
    
        Debug.Log($"Nowy pożar w miejscu: {wolnePunkty[losowyIndeks].name}");
    
        wolnePunkty[losowyIndeks].SetActive(true);

    }
}
