using System;
using UnityEngine;

public class CameraTransitionManager : MonoBehaviour
{
    public static CameraTransitionManager Instance { get; private set; }

    [Header("Cameras")]
    [SerializeField] GameObject hullViewCamera;
    [SerializeField] GameObject deckViewCamera;
    [SerializeField] GameObject topViewCamera;
    [SerializeField] GameObject backViewCamera;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject startCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject ShipAttackCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Ship.OnEquipmentChange += Ship_OnEquipmentChange;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    EnableAttackCamera();
        //}
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

    public void EnableAttackCamera()
    {
        DisableAllCameras();

        ShipAttackCamera.SetActive(true);
    }

    public void EnableMainCamera()
    {
        DisableAllCameras();

        mainCamera.SetActive(true);
        player.SetActive(true);
    }

    void DisableAllCameras()
    {
        mainCamera.SetActive(false);
        startCamera.SetActive(false);
        hullViewCamera.SetActive(false);
        deckViewCamera.SetActive(false);
        topViewCamera.SetActive(false);
        backViewCamera.SetActive(false);
        ShipAttackCamera.SetActive(false);
        player.SetActive(false);
    }
}
