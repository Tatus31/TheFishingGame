using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveShip : MonoBehaviour
{
    [SerializeField] Transform leavePoint;
    [SerializeField] LayerMask layerMask;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = leavePoint.position;
    }
}
