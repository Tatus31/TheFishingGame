using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplayController : MonoBehaviour
{
    bool isHiden = true;
    [SerializeField] GameObject inventoryUIObj;
    [SerializeField] GameObject shipEquipmentUIObj;
    [SerializeField] LayerMask interactionLayerUI;
    CameraLook cameraLook;

    private void Start()
    {
        inventoryUIObj.SetActive(false);
        shipEquipmentUIObj.SetActive(false);

        cameraLook = FindObjectOfType<CameraLook>();
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
        cameraLook.enabled = false;
    }

    void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isHiden = true;
        inventoryUIObj.SetActive(false);
        shipEquipmentUIObj.SetActive(false);
        cameraLook.enabled = true;
    }
}
