using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayController : MonoBehaviour
{
    bool isHiden = true;
    [SerializeField] GameObject inventoryUIObj;
    [SerializeField] CameraLook cameraLookScript;

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
        cameraLookScript.enabled = false;
    }

    void HideInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isHiden = true;
        inventoryUIObj.SetActive(false);
        cameraLookScript.enabled = true;
    }
}
