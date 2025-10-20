using System.Collections.Generic;
using System;
using UnityEngine;

public class ShipRepairPoints : MonoBehaviour
{
    public static ShipRepairPoints Instance;

    public static event EventHandler OnHoleCreate;

    public event Action<int> OnRepairPointsChanged;

    [System.Serializable]
    public class RepairPoint
    {
        public Vector3 position;
        public Vector3 rotation;
        public bool isUsed = false;
        public int damageValue = 0;
    }

    [SerializeField] List<RepairPoint> repairPoints = new List<RepairPoint>();
    [SerializeField] GameObject obj;
    List<GameObject> holePool = new();
    [SerializeField] int damageThreshold = 20;

    public List<RepairPoint> RepairPoints {  get { return repairPoints; } }

    Queue<int> pendingHoleDamages = new();

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;
    }

    private void Start()
    {
        foreach (var point in repairPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(point.position);
            Quaternion worldRotation = transform.rotation * Quaternion.Euler(point.rotation);
            GameObject hole = Instantiate(obj, worldPoint, worldRotation, transform);
            hole.SetActive(false);            
            holePool.Add(hole);
        }

        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
    }

    void Update()
    {
        if (pendingHoleDamages.Count > 0)
        {
            int dmg = pendingHoleDamages.Dequeue();
            SpawnHole(dmg);
        }
    }

    public void SpawnHole(int damagePerHole = 0)
    {
        for (int i = 0; i < repairPoints.Count; i++)
        {
            var point = repairPoints[i];
            if (!point.isUsed)
            {
                point.isUsed = true;
                point.damageValue = damagePerHole;

                holePool[i].SetActive(true);         
                OnRepairPointsChanged?.Invoke(GetUsedRepairPoints());
                OnHoleCreate?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    void ShipDamage_OnDamageTaken(object sender, int e)
    {
        int damageTaken = Mathf.Abs(e);
        int holesToSpawn = damageTaken / damageThreshold;

        if (holesToSpawn > 0)
        {
            int damagePerHole = damageTaken / holesToSpawn;
            for (int i = 0; i < holesToSpawn; i++) pendingHoleDamages.Enqueue(damagePerHole);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"{damageTaken} too small for holes");
#endif
        }
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
        RepairPoint pointToReset = repairPoints.Find(point => Vector3.Distance(transform.TransformPoint(point.position), position) < 0.1f);

        if (pointToReset != null)
        {
            int healthToRestore = pointToReset.damageValue;
            pointToReset.isUsed = false;
            pointToReset.damageValue = 0;

            if (ShipDamage.Instance != null)
            {
                ShipDamage.Instance.RestoreHealth(healthToRestore);
            }

            OnRepairPointsChanged?.Invoke(GetUsedRepairPoints());
        }
    }

    public void ResetAllRepairPoints()
    {
        int totalHealthToRestore = 0;

        foreach (var point in repairPoints)
        {
            if (point.isUsed)
            {
                totalHealthToRestore += point.damageValue;
                point.isUsed = false;
                point.damageValue = 0;
            }
        }

        OnRepairPointsChanged?.Invoke(GetUsedRepairPoints());
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