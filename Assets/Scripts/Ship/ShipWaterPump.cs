using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWaterPump : MonoBehaviour
{
    [SerializeField] LayerMask waterPumpLayerMask;

    ShipRepairPoints shipRepairPoints;

    int currentRepairPoints;

    private void Start()
    {
        shipRepairPoints = GetComponentInParent<ShipRepairPoints>();

        shipRepairPoints.OnRepairPointsChanged += ShipRepairPoints_OnRepairPointsChanged;
    }

    private void ShipRepairPoints_OnRepairPointsChanged(int points)
    {
        currentRepairPoints = points;
    }

    private void Update()
    {
        if(MouseWorldPosition.GetInteractable(waterPumpLayerMask) && Input.GetKey(KeyCode.E))
        {
            Input.GetMouseButtonDown(0);
            Debug.Log("using waterPump");

            ChangeWaterLevelUnderDeck.Instance.LinearlyDecreaseWaterLevel();
        }
    }
}
