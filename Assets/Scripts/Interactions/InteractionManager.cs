using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public enum EquipedTool
    {
        FishingRod,
        Harpoon,
        DivingSuit,
        Empty
    }

    [System.Serializable]
    public class ToolConfiguration
    {
        public GameObject handObject;
        public LayerMask interactionMask;
        public bool enableFishingState;
    }

    public static InteractionManager Instance;

    [Header("Tool Configurations")]
    [SerializeField] private ToolConfiguration fishingConfig;
    [SerializeField] private ToolConfiguration harpoonConfig;
    [SerializeField] private ToolConfiguration divingSuitConfig;
    [SerializeField] private FishingStateManager fishingStateManager;

    private EquipedTool currentTool = EquipedTool.Empty;

    public EquipedTool CurrentTool => currentTool;
    public bool HasHarpoon => currentTool == EquipedTool.Harpoon;
    public bool IsInDivingSuit => currentTool == EquipedTool.DivingSuit;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"There already is a {Instance.name} in the scene");

        Instance = this;
    }

    private void Start()
    {
        EquipTool(EquipedTool.DivingSuit);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (MouseWorldPosition.GetInteractable(harpoonConfig.interactionMask))
        {
            EquipTool(EquipedTool.Harpoon);
        }
        else if (MouseWorldPosition.GetInteractable(divingSuitConfig.interactionMask))
        {
            EquipTool(EquipedTool.DivingSuit);
        }
        else if (MouseWorldPosition.GetInteractable(fishingConfig.interactionMask))
        {
            EquipTool(EquipedTool.FishingRod);
        }
    }

    public void EquipTool(EquipedTool newTool)
    {
        fishingConfig.handObject.SetActive(false);
        harpoonConfig.handObject.SetActive(false);
        divingSuitConfig.handObject.SetActive(false);

        switch (newTool)
        {
            case EquipedTool.FishingRod:
                fishingConfig.handObject.SetActive(true);
                fishingStateManager.enabled = true;
                break;

            case EquipedTool.Harpoon:
                harpoonConfig.handObject.SetActive(true);
                fishingStateManager.enabled = false;
                break;

            case EquipedTool.DivingSuit:
                divingSuitConfig.handObject.SetActive(true);
                fishingStateManager.enabled = false;
                break;

            case EquipedTool.Empty:
                fishingStateManager.enabled = false;
                break;
        }

        currentTool = newTool;
    }

    public bool IsToolEquipped(EquipedTool tool) => currentTool == tool;
}