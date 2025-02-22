using System;
using System.Collections;
using UnityEngine;

public class StartFire : MonoBehaviour
{
    [SerializeField] GameObject fireVFX;
    [SerializeField] float fireTickInterval = 1.0f;

    ElectricalDevice electricalDevice;
    ShipDamage shipDamage;

    bool isOnFire;
    public bool IsOnFire {  get { return isOnFire; } set {  isOnFire = value; } }

    private void Start()
    {
        fireVFX.SetActive(false);
        isOnFire = false;

        electricalDevice = GetComponent<ElectricalDevice>();
        shipDamage = ShipDamage.Instance;

        ElectricalDevice.OnDegradation += ElectricalDevice_OnDegradation;
    }

    private void ElectricalDevice_OnDegradation(object sender, EventArgs e)
    {
        if (electricalDevice.CurrentDegradation == ElectricalDevice.DegradationCondition.Bad)
        {
            Debug.Log("start fire");
            fireVFX.SetActive(true);
            isOnFire = true;
            StartCoroutine(FireTickDamage());
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
}
