using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class IncreaseWaterLevel : MonoBehaviour
{
    public static IncreaseWaterLevel Instance;

    public event EventHandler<bool> OnSinkingShip;

    [SerializeField] Vector3 maxWaterLevel;
    [SerializeField] float baseRiseSpeed = 0.2f; 
    [SerializeField] float riseSpeedMultiplier = 1.0f;

    ShipRepairPoints shipRepairPoints;

    float currentRepairPoints;

    Vector3 localMaxWaterLevel;

    bool shipSank = false;
    public bool ShipSank {  get { return shipSank; } set {  shipSank = value; } }

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;
    }

    private void Start()
    {
        shipRepairPoints = GetComponentInParent<ShipRepairPoints>();

        localMaxWaterLevel = transform.TransformPoint(maxWaterLevel);

        shipRepairPoints.OnRepairPointsChanged += ShipRepairPoints_OnRepairPointsChanged;
    }

    private void Update()
    {
        if(currentRepairPoints > 0 && !shipSank)
            LinearlyIncreaseWaterLevel();
    }

    private void ShipRepairPoints_OnRepairPointsChanged(int points)
    {
        currentRepairPoints = points;
    }

    void LinearlyIncreaseWaterLevel()
    {
        float currentRiseSpeed = baseRiseSpeed * (1 + (currentRepairPoints - 1) * riseSpeedMultiplier);

        Vector3 currentWaterLevelPos = transform.position;

        currentWaterLevelPos.y = Mathf.MoveTowards(currentWaterLevelPos.y, localMaxWaterLevel.y, currentRiseSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, localMaxWaterLevel) <= 0.01f)
        {
            LeaveUnderDeck.Instance.MovePlayerManually();
            shipSank = true;
            OnSinkingShip?.Invoke(this, shipSank);
        }

        transform.position = currentWaterLevelPos;

        //transform.position.y = Mathf.Lerp(transform.position.y, localMaxWaterLevel.y, 1);
    }

    private void OnDestroy()
    {
        if (shipRepairPoints != null)
            shipRepairPoints.OnRepairPointsChanged -= ShipRepairPoints_OnRepairPointsChanged;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(localMaxWaterLevel, new Vector3(3, 0.1f, 8));
    }
}
