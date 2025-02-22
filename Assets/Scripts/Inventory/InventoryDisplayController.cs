using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplayController : MonoBehaviour
{
    bool isHiden = true;
    [SerializeField] GameObject player;
    [SerializeField] GameObject mainCamera;
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
            mainCamera.SetActive(false);
            shipEquipmentUIObj.SetActive(true);
            player.SetActive(false);
        }
        cameraLook.enabled = false;
    }

    void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isHiden = true;
        inventoryUIObj.SetActive(false);
        if (shipEquipmentUIObj.activeSelf)
        {
            shipEquipmentUIObj.SetActive(false);
            mainCamera.SetActive(true);
            player.SetActive(true);
        }
        cameraLook.enabled = true;
    }
}
