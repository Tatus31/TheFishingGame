using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    ShipMovement movement;
    void Start()
    {
        movement = FindObjectOfType<ShipMovement>();
    }


    void Update()
    {
        if(movement.IsControllingShip)
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, movement.CurrentWheelRotation, transform.localRotation.z);
    }
}
