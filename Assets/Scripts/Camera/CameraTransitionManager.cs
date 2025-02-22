using System;
using UnityEngine;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] GameObject hullViewCamera;
    [SerializeField] GameObject deckViewCamera;
    [SerializeField] GameObject topViewCamera;
    [SerializeField] GameObject backViewCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject startCamera;
    [SerializeField] GameObject Player;

    private void Start()
    {
        Ship.OnEquipmentChange += Ship_OnEquipmentChange;
    }

    private void Ship_OnEquipmentChange(object sender, ItemType e)
    {
        DisableAllCameras();

        switch (e)
        {
            case ItemType.Detection:
                topViewCamera.SetActive(true);
                break;
            case ItemType.Hull:
                hullViewCamera.SetActive(true);
                break;
            case ItemType.Propeller:
                backViewCamera.SetActive(true);
                break;
            case ItemType.Storage:
                deckViewCamera.SetActive(true);
                break;
            default:
                startCamera.SetActive(true);
                break;
        }
    }

    void DisableAllCameras()
    {
        mainCamera.SetActive(false);
        startCamera.SetActive(false);
        hullViewCamera.SetActive(false);
        deckViewCamera.SetActive(false);
        topViewCamera.SetActive(false);
        backViewCamera.SetActive(false);
        Player.SetActive(false);
    }
}
