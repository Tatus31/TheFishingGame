using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    public static Testing Instance;

    [SerializeField] LayerMask Interactable;
    [SerializeField] LayerMask InteractableElectronic;
    [SerializeField] GameObject repairBarObject;

    FishingStateManager fishingStateManager;

    public bool isRepairing = false;

    private Camera mainCamera;
    [SerializeField] GameObject backgroundObject; // Reference to the background object

    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main; // Cache the main camera
    }

    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && MouseWorldPosition.GetInteractable(InteractableElectronic))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isRepairing = true;
        }

        if (isRepairing && MouseWorldPosition.GetObjectOverMouse("Blue_Pipe") && InputManager.Instance.IsLeftMouseButtonHeld())
        {
            GameObject obj = MouseWorldPosition.GetObjectOverMouse("Blue_Pipe");

            if (obj != null)
            {
                // Convert mouse position to world space
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0; // Ensure the z position is correct (assuming 2D)

                // Adjust the position relative to the background
                Vector3 relativePosition = backgroundObject.transform.InverseTransformPoint(mouseWorldPosition);
                obj.transform.position = backgroundObject.transform.TransformPoint(relativePosition);
            }
        }

        if (MouseWorldPosition.GetInteractable(Interactable) && InputManager.Instance.IsLeftMouseButtonPressed())
        {
            repairBarObject.SetActive(true);
        }
    }
}