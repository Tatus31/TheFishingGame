using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterShip : MonoBehaviour
{
    [SerializeField] Transform enterPoint;
    [SerializeField] LayerMask layerMask;
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = enterPoint.position;
    }
}
