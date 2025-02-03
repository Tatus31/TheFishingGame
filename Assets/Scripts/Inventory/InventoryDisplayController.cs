using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour
{
    bool isHiden = true;
    [SerializeField] GameObject inventoryUIObj;
    [SerializeField] GameObject shipEquipmentUIObj;
    [SerializeField] LayerMask interactionLayerUI;
    [SerializeField] CameraLook cameraLookScript;

    private void Start()
    {
        inventoryUIObj.SetActive(false);
        shipEquipmentUIObj.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            if (isHiden)
            {
                DisplayInventory();
            }
            else
            {
                HideInventory();
            }
        }
    }

    void DisplayInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isHiden = false;
        inventoryUIObj.SetActive(true);
        if (MouseWorldPosition.GetInteractable(interactionLayerUI))
        {
            shipEquipmentUIObj.SetActive(true);
        }
        cameraLookScript.enabled = false;
    }

    void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isHiden = true;
        inventoryUIObj.SetActive(false);
        shipEquipmentUIObj.SetActive(false);
        cameraLookScript.enabled = true;
    }
}
