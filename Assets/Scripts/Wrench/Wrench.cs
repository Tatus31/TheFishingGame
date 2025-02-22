using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Wrench : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    private void Update()
    {
        if (MouseWorldPosition.GetInteractable(layerMask) && InputManager.Instance.IsLeftMouseButtonPressed())
        {
            ShipDamage.Instance.RestoreHealth(5);
        }
    }
}
