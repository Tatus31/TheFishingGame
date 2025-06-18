using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthVisualiser : MonoBehaviour
{
    [SerializeField] List<GameObject> bloodFlowObjs = new List<GameObject>();

    private void Start()
    {
        foreach (var obj in bloodFlowObjs)
        {
            obj.SetActive(false);
        }
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
        ShipDamage.Instance.OnRestoreDamage += ShipDamage_OnRestoreDamage;
    }

    private void ShipDamage_OnRestoreDamage(object sender, int healAmount)
    {
        int currentHealth = ShipDamage.Instance.GetModifiedStatValue(Stats.Health);
        int maxHealth = ShipDamage.Instance.GetPermanentSavedStatValue(Stats.Health);
        float healthRatio = (float)currentHealth / maxHealth;

        if (healthRatio > 1f / 3f && bloodFlowObjs.Count > 2)
        {
            bloodFlowObjs[2].SetActive(false);
        }

        if (healthRatio > 2f / 3f && bloodFlowObjs.Count > 1)
        {
            bloodFlowObjs[1].SetActive(false);
        }

        if (healthRatio >= 1f && bloodFlowObjs.Count > 0)
        {
            bloodFlowObjs[0].SetActive(false);
        }
    }

    private void ShipDamage_OnDamageTaken(object sender, int damage)
    {
        int currentHealth = ShipDamage.Instance.GetModifiedStatValue(Stats.Health);
        int maxHealth = ShipDamage.Instance.GetPermanentSavedStatValue(Stats.Health);
        float healthRatio = (float)currentHealth / maxHealth;

        if (healthRatio < 1f)
        {
            if (bloodFlowObjs.Count > 0)
                bloodFlowObjs[0].SetActive(true);
        }
        if (healthRatio <= 2f / 3f)
        {
            if (bloodFlowObjs.Count > 1)
                bloodFlowObjs[1].SetActive(true);
        }
        if (healthRatio <= 1f / 3f)
        {
            if (bloodFlowObjs.Count > 2)
                bloodFlowObjs[2].SetActive(true);
        }
    }
}