using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveShip : MonoBehaviour
{
    [SerializeField] Transform leaveshipPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = leaveshipPoint.position;
    }
}
