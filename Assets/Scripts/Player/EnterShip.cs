using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterShip : MonoBehaviour
{
    [SerializeField] Transform enterShipPoint;
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = enterShipPoint.position;
    }
}
