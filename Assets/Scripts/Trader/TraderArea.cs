using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TraderArea : MonoBehaviour
{
    public static bool IsInsideTraderArea;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagHolder.PLAYER))
        {
            IsInsideTraderArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagHolder.PLAYER))
        {
            IsInsideTraderArea = false;
        }
    }
}
