using System;
using UnityEngine;

public class ElectricalDevice : MonoBehaviour
{
    public static event EventHandler OnDegradation;

    [Serializable]
    public class ElectricalDeviceStats
    {
        [Range(0, 100)] public int condition;
        public int repairBonus = 30;
        public float degradationTime = 5;
        public Stats deviceStats;
        public int statDegradation;
    }

    public enum DegradationCondition
    {
        Perfect,
        Ok,
        Average,
        Bad,
    }

    [SerializeField] ElectricalDeviceStats deviceStats;
    [SerializeField] DegradationCondition currentDegradation;

    public ElectricalDeviceStats DeviceStats => deviceStats;
    public DegradationCondition CurrentDegradation => currentDegradation;

    private void Start()
    {
        GlobalCooldown.Instance.StartCooldown(deviceStats.degradationTime);
        currentDegradation = DegradationCondition.Perfect;
    }

    private void Update()
    {
        if (!GlobalCooldown.Instance.IsInCooldown())
        {
            deviceStats.condition = Mathf.Max(0, deviceStats.condition - 1);
            GlobalCooldown.Instance.StartCooldown(deviceStats.degradationTime);
            UpdateDegradationCondition();
        }
    }

    public void RepairDevice()
    {
        deviceStats.condition = Mathf.Min(100, deviceStats.condition + deviceStats.repairBonus);
        UpdateDegradationCondition();
    }

    public void RepairDevice(int repairValue)
    {
        deviceStats.condition = repairValue;
        UpdateDegradationCondition();
    }

    void UpdateDegradationCondition()
    {
        DegradationCondition newDegradation = GetDegradationCondition(deviceStats.condition);

        if (newDegradation != currentDegradation)
        {
            currentDegradation = newDegradation;
            OnDegradation?.Invoke(this, EventArgs.Empty);
        }
    }

    DegradationCondition GetDegradationCondition(int condition)
    {
        return condition switch
        {
            >= 90 => DegradationCondition.Perfect,
            >= 60 => DegradationCondition.Ok,
            >= 50 => DegradationCondition.Average,
            >= 30 => DegradationCondition.Bad,
            _ => DegradationCondition.Bad
        };
    }
}
