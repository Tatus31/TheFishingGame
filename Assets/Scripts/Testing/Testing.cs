using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] Transform testingSphere;

    void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonPressed())
        {
            Debug.Log("leftmouse");
        }

        testingSphere.position = MouseWorldPosition.GetMouseWorldPosition(10f);
    }
}
