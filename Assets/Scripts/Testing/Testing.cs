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
    [SerializeField] CameraLook cameraLook;


    FishingStateManager fishingStateManager;
    ShipMovement shipMovement;
    public bool isRepairing = false;

    private Camera mainCamera;
    [SerializeField] GameObject backgroundObject;


    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        shipMovement = FindObjectOfType<ShipMovement>();
    }

    private void Start()
    {
    }


    void Update()
    {
        //GameObject objCanv = MouseWorldPosition.GetObjectOverMouse(MouseWorldPosition.Instance.InteractableMask);

        //if (objCanv != null )
        //{
        //    Debug.Log("Object over mouse: " + objCanv.name);

        //    if (MouseWorldPosition.GetInteractable(MouseWorldPosition.Instance.InteractableMask))
        //    {
        //        GameObject interactObj = objCanv.GetComponentInChildren<Canvas>().gameObject;
        //        Debug.Log("Interactable object found: " + interactObj.name);
        //        interactObj.transform.GetChild(0).gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        GameObject interactObj = objCanv.GetComponentInChildren<Canvas>().gameObject;
        //        interactObj.transform.GetChild(0).gameObject.SetActive(false);
        //    }

        //    if (shipMovement.IsControllingShip)
        //    {
        //        GameObject interactObj = objCanv.GetComponentInChildren<Canvas>().gameObject;
        //        interactObj.transform.GetChild(0).gameObject.SetActive(false);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.E) && MouseWorldPosition.GetInteractable(InteractableElectronic) && !isRepairing)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isRepairing = true;
            cameraLook.Sensitivity = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.E) && isRepairing) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isRepairing = false;
            cameraLook.Sensitivity = 1f;
        }

        if (isRepairing && MouseWorldPosition.GetObjectOverMouse("Blue_Pipe") && InputManager.Instance.IsLeftMouseButtonHeld())
        {
            GameObject obj = MouseWorldPosition.GetObjectOverMouse("Blue_Pipe");

            if (obj != null)
            {
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;

                Vector3 relativePosition = backgroundObject.transform.InverseTransformPoint(mouseWorldPosition);
                obj.transform.position = backgroundObject.transform.TransformPoint(relativePosition);
            }
        }

        if (MouseWorldPosition.GetInteractable(Interactable) && Input.GetKeyDown(KeyCode.E))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraLook.Sensitivity = 0f;

            repairBarObject.SetActive(true);
        }
    }
}