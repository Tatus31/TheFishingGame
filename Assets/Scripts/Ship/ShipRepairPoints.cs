using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRepairPoints : MonoBehaviour
{
    [System.Serializable]
    public class RepairPoint
    {
        public Vector3 position;
        public Vector3 rotation;
    }

    [SerializeField]
    List<RepairPoint> repairPoints = new List<RepairPoint>();
    [SerializeField]
    GameObject obj;

    Vector3 worldPoint;
    Quaternion worldRotation;

    private void Start()
    {
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;

        //foreach (var point in repairPoints)
        //{

        //}
    }

    void SpawnHole()
    {
        foreach (var point in repairPoints)
        {
            worldPoint = transform.TransformPoint(point.position);
            worldRotation = transform.rotation * Quaternion.Euler(point.rotation);
            Instantiate(obj, worldPoint, worldRotation);
        }
    }

    void ShipDamage_OnDamageTaken(object sender, int e)
    {
        SpawnHole();
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