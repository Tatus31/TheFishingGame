using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] Transform testingSphere;

    void Update()
    {
        //if (InputManager.Instance.IsLeftMouseButtonPressed())
        //{
        //    Debug.Log("leftmouse click");
        //}
        //if (InputManager.Instance.IsLeftMouseButtonHeld())
        //{
        //    Debug.Log("leftmouse held");
        //}
        //if (InputManager.Instance.IsLeftMouseButtonReleased())
        //{
        //    Debug.Log("leftmouse release");
        //}

        testingSphere.position = MouseWorldPosition.GetMouseWorldPosition();
    }
}
