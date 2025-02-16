using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependantCameraLook : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField][Range(0.1f, 1)] float sensitivity = 1f;

    float sensMultiplier = 5f;

    Vector2 currentMouseDelta;
    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        currentMouseDelta = InputManager.Instance.GetMouseDelta() * sensitivity * Time.fixedDeltaTime * sensMultiplier;
    }

    void LateUpdate()
    {
        MouseLook();
    }

    void MouseLook()
    {
        float desiredX;

        Vector3 rot = transform.localRotation.eulerAngles;
        desiredX = rot.y + currentMouseDelta.x;

        xRotation -= currentMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, desiredX, transform.rotation.z);
    }
}
