using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ChangeWaterLevelUnderDeck : MonoBehaviour
{
    public static ChangeWaterLevelUnderDeck Instance;

    public event EventHandler<bool> OnSinkingShip;
    public event EventHandler<bool> OnShipCatchingWater;

    [Header("Water Positions")]
    [SerializeField] Vector3 maxWaterLevel;
    [SerializeField] Vector3 minWaterLevel;

    [Header("Water change values")]
    [SerializeField] float baseSpeed = 0.01f; 
    [SerializeField] float riseSpeedMultiplier = 0.01f;
    [SerializeField] float lowerSpeedMultiplier = 0.01f;

    ShipRepairPoints shipRepairPoints;

    float currentRepairPoints;

    Vector3 localMaxWaterLevel;
    Vector3 localMinWaterLevel;

    bool shipSank = false;
    bool isWaterUnderDeck = false;
    bool isInvincible;

    public bool ShipSank {  get { return shipSank; } set {  shipSank = value; } }
    public bool IsWaterUnderDeck { get {  return isWaterUnderDeck; } set {  isWaterUnderDeck = value; } }
    public bool IsInvincible { get { return isInvincible; } set { isInvincible = value; } }

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
        localMinWaterLevel = transform.TransformPoint(minWaterLevel);

        shipRepairPoints.OnRepairPointsChanged += ShipRepairPoints_OnRepairPointsChanged;
    }

    private void FixedUpdate()
    {
        if (isInvincible)
            return;

        if (currentRepairPoints > 0 && !shipSank)
            LinearlyIncreaseWaterLevel();
    }

    private void ShipRepairPoints_OnRepairPointsChanged(int points)
    {
        currentRepairPoints = points;
    }

    public void LinearlyIncreaseWaterLevel()
    {
        float currentRiseSpeed = baseSpeed * (1 + (currentRepairPoints - 1) * riseSpeedMultiplier);

        Vector3 currentWaterLevelPos = transform.position;

        currentWaterLevelPos.y = Mathf.MoveTowards(currentWaterLevelPos.y, localMaxWaterLevel.y, currentRiseSpeed * Time.fixedDeltaTime);

        isWaterUnderDeck = true;
        OnShipCatchingWater?.Invoke(this, isWaterUnderDeck);

        if (Vector3.Distance(transform.position, localMaxWaterLevel) <= 0.01f)
        {
            LeaveUnderDeck.Instance.MovePlayerManually();

            if (!isInvincible)
            {
                shipSank = true;
                OnSinkingShip?.Invoke(this, shipSank);
            }
        }

        transform.position = currentWaterLevelPos;

        //transform.position.y = Mathf.Lerp(transform.position.y, localMaxWaterLevel.y, 1);
    }

    public void LinearlyDecreaseWaterLevel()
    {
        float currentDropSpeed = baseSpeed / (1 + (currentRepairPoints - 1) * lowerSpeedMultiplier);
        Vector3 currentWaterLevelPos = transform.position;
        currentWaterLevelPos.y = Mathf.MoveTowards(currentWaterLevelPos.y, localMinWaterLevel.y, currentDropSpeed * Time.fixedDeltaTime);
        transform.position = currentWaterLevelPos;
    }

    //public void LinearlyDecreaseWaterLevel(int repairPoints)
    //{
    //    float currentDropSpeed = baseSpeed * (1 / (currentRepairPoints - 1) * speedMultiplier);
    //    Vector3 currentWaterLevelPos = transform.position;
    //    currentWaterLevelPos.y = Mathf.MoveTowards(currentWaterLevelPos.y, localMinWaterLevel.y, currentDropSpeed * Time.fixedDeltaTime);

    //    if (Vector3.Distance(transform.position, localMinWaterLevel) <= 0.01f)
    //    {
    //        Debug.Log("water level down");
    //    }

    //    transform.position = currentWaterLevelPos;
    //}

    private void OnDestroy()
    {
        if (shipRepairPoints != null)
            shipRepairPoints.OnRepairPointsChanged -= ShipRepairPoints_OnRepairPointsChanged;
    }

    private void OnDrawGizmos()
    {
        //localMaxWaterLevel = transform.TransformPoint(maxWaterLevel);
        //localMinWaterLevel = transform.TransformPoint(minWaterLevel);

        Gizmos.DrawWireCube(localMaxWaterLevel, new Vector3(3, 0.1f, 8));
        Gizmos.color = UnityEngine.Color.yellow;
        Gizmos.DrawWireCube(localMinWaterLevel, new Vector3(3, 0.1f, 8));
    }
}
