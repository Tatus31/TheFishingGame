using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairPoints : MonoBehaviour
{
    public event Action<int> OnRepairPointsChanged;

    [System.Serializable]
    public class RepairPoint
    {
        public Vector3 position;
        public Vector3 rotation;
        public bool isUsed = false;
    }

    [SerializeField] List<RepairPoint> repairPoints = new List<RepairPoint>();
    [SerializeField] GameObject obj;
    [SerializeField] int damageThreshold = 20;

    private void Start()
    {
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
    }

    private void Update()
    {
        foreach (var pair in repairPoints)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(pair.isUsed == false)
                SpawnHole();
            }

        }
    }

    void SpawnHole()
    {
        RepairPoint unusedPoint = repairPoints.Find(point => !point.isUsed);

        if (unusedPoint != null)
        {
            Vector3 worldPoint = transform.TransformPoint(unusedPoint.position);
            Quaternion worldRotation = transform.rotation * Quaternion.Euler(unusedPoint.rotation);
            Instantiate(obj, worldPoint, worldRotation, transform);
            unusedPoint.isUsed = true;

            OnRepairPointsChanged?.Invoke(GetUsedRepairPoints());
        }
    }

    void ShipDamage_OnDamageTaken(object sender, int e)
    {
        int damageTaken = e;
        damageTaken = Mathf.Abs(damageTaken);
        Debug.Log($"Damage taken: {damageTaken} based on repairpoints");

        int holesToSpawn = damageTaken / damageThreshold;
        Debug.Log($"Spawning {holesToSpawn} holes");

        for (int i = 0; i < holesToSpawn; i++)
        {
            SpawnHole();
        }
        Debug.Log(GetUsedRepairPoints());
    }

    public int GetUsedRepairPoints()
    {
        int repairPointCount = 0;

        foreach (var point in repairPoints)
        {
            if (point.isUsed)
                repairPointCount++;
        }

        return repairPointCount;
    }

    public void ResetRepairPointAtPosition(Vector3 position)
    {
        RepairPoint pointToReset = repairPoints.Find(point =>
            Vector3.Distance(transform.TransformPoint(point.position), position) < 0.1f);

        if (pointToReset != null)
        {
            pointToReset.isUsed = false;
            OnRepairPointsChanged?.Invoke(GetUsedRepairPoints());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach (var point in repairPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(point.position);
            Quaternion worldRotation = transform.rotation * Quaternion.Euler(point.rotation);

            Gizmos.DrawWireSphere(worldPoint, 0.1f);

            Vector3 forward = worldRotation * Vector3.right * 0.2f;
            Gizmos.DrawRay(worldPoint, forward);
        }
    }
}