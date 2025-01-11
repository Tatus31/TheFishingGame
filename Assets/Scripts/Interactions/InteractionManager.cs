using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    [SerializeField] GameObject fishingHands;
    [SerializeField] GameObject divingSuitHands;
    [SerializeField] GameObject harpoonHands;
    [SerializeField] FishingStateManager fishingStateManager;

    bool isInDivingSuit;
    bool hasHarpoon;

    [SerializeField] LayerMask HookMask;
    [SerializeField] LayerMask SuitMask;


    public bool IsInDivingSuit { get { return isInDivingSuit; } private set { isInDivingSuit = value; } }

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there already is a {Instance.name} in the scene");

        Instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //TODO: Change this shit 

        //if(MouseWorldPosition.GetInteractable() && Input.GetKeyDown(KeyCode.E))
        //{

        //}

        if (MouseWorldPosition.GetInteractable(HookMask) && !hasHarpoon && Input.GetKeyDown(KeyCode.E))
        {
            hasHarpoon = true;
            harpoonHands.SetActive(true);
            divingSuitHands.SetActive(false);
            fishingHands.SetActive(false);
            fishingStateManager.enabled = false;
        }
        else if (MouseWorldPosition.GetInteractable(HookMask) && hasHarpoon && Input.GetKeyDown(KeyCode.E))
        {
            hasHarpoon = false;
            harpoonHands.SetActive(false);
            divingSuitHands.SetActive(false);
            fishingHands.SetActive(true);
            fishingStateManager.enabled = true;
        }

        if (MouseWorldPosition.GetInteractable(SuitMask) && !isInDivingSuit && Input.GetKeyDown(KeyCode.E))
        {
            isInDivingSuit = true;
            divingSuitHands.SetActive(true);
            fishingHands.SetActive(false);
            harpoonHands.SetActive(false);
            fishingStateManager.enabled = false;
        }
        else if (MouseWorldPosition.GetInteractable(SuitMask) && isInDivingSuit && Input.GetKeyDown(KeyCode.E))
        {
            isInDivingSuit = false;
            divingSuitHands.SetActive(false);
            fishingHands.SetActive(true);
            harpoonHands.SetActive(false);
            fishingStateManager.enabled = true;
        }
    }

    void ChangeHands()
    {

    }
}
