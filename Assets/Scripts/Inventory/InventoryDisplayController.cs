using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplayController : MonoBehaviour
{
    bool isHidden = true;

    [SerializeField] GameObject player;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject inventoryUIObj;
    [SerializeField] GameObject shipEquipmentUIObj;
    [SerializeField] GameObject craftingUIObj;

    [SerializeField] LayerMask shipEquipmentLayerUI;
    [SerializeField] LayerMask craftingLayerUI;

    private void Start()
    {
        inventoryUIObj.SetActive(false);
        shipEquipmentUIObj.SetActive(false);
        craftingUIObj.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.GetInventoryInputDown())
        {
            if (isHidden)
                DisplayInventory();
            else
                HideInventory();
        }
    }

    void DisplayInventory()
    {
        isHidden = false;
        inventoryUIObj.SetActive(true);

        CameraLook.LockCamera(true);

        if (MouseWorldPosition.GetInteractable(craftingLayerUI))
        {
            craftingUIObj.SetActive(true);
        }

        if (MouseWorldPosition.GetInteractable(shipEquipmentLayerUI))
        {
            mainCamera.SetActive(false);
            shipEquipmentUIObj.SetActive(true);
            player.SetActive(false);
        }
    }

    void HideInventory()
    {
        var ui = inventoryUIObj.GetComponent<UserInterface>();
        
        if (ui)
            ui.DisableToolTip();

        isHidden = true;
        inventoryUIObj.SetActive(false);

        CameraLook.LockCamera(false);

        if (shipEquipmentUIObj.activeSelf)
        {
            shipEquipmentUIObj.SetActive(false);
            mainCamera.SetActive(true);
            player.SetActive(true);
        }

        if (craftingUIObj.activeSelf)
        {
            craftingUIObj.SetActive(false);
        }
    }
}
