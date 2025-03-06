using System;
using System.Collections;
using UnityEngine;

public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] GameObject sparksVFX;
    [SerializeField] float fireTickInterval = 1.0f;

    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;

    bool isSparking;

    bool isOnFire;
    public bool IsOnFire {  get { return isOnFire; } set {  isOnFire = value; } }

    private void Start()
    {
        fireVFX.SetActive(false);
        sparksVFX.SetActive(false);

        isOnFire = false;

        electricalDevice = GetComponent<ElectricalDevice>();
        shipDamage = ShipDamage.Instance;

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
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
            FireAction();
        }
    }

    IEnumerator FireTickDamage()
    {
        while (isOnFire)
        {
            shipDamage.TakeFireDamage();
            yield return new WaitForSeconds(fireTickInterval);
        }

        fireVFX.SetActive(false);
    }

    void FireAction()
    {
        Debug.Log("start fire");
        fireVFX.SetActive(true);
        isOnFire = true;
        StartCoroutine(FireTickDamage());
    }
}
