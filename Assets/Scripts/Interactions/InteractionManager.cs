using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public enum EquipedTool
    {
        Harpoon,
        EmptyHands,
        Empty,
        FireExtinguisher,
        Wrench
    }

    [System.Serializable]
    public class ToolConfiguration
    {
        public GameObject handObject;
        public LayerMask interactionMask;
    }

    [Header("Tool Configurations")]
    [SerializeField] ToolConfiguration fishingConfig;
    [SerializeField] ToolConfiguration harpoonConfig;
    [SerializeField] ToolConfiguration emptyHandsConfig;
    [SerializeField] ToolConfiguration fireExtinguisherConfig;
    [SerializeField] ToolConfiguration WrenchConfig;

    EquipedTool currentTool = EquipedTool.Empty;

    public EquipedTool CurrentTool => currentTool;
    public bool HasHarpoon => currentTool == EquipedTool.Harpoon;
    public bool AreHandsEmpty => currentTool == EquipedTool.EmptyHands;
    public bool hasFireExtinguisher => currentTool == EquipedTool.FireExtinguisher;
    public bool hasWrench => currentTool == EquipedTool.Wrench;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"There already is a {Instance.name} in the scene");

        Instance = this;
    }

    private void Start()
    {
        EquipTool(EquipedTool.EmptyHands);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (MouseWorldPosition.GetInteractable(harpoonConfig.interactionMask))
        {
            EquipTool(EquipedTool.Harpoon);
        }
        else if (MouseWorldPosition.GetInteractable(emptyHandsConfig.interactionMask))
        {
            EquipTool(EquipedTool.EmptyHands);
        }
        else if (MouseWorldPosition.GetInteractable(fireExtinguisherConfig.interactionMask))
        {
            EquipTool(EquipedTool.FireExtinguisher);
        }
        else if (MouseWorldPosition.GetInteractable(WrenchConfig.interactionMask))
        {
            EquipTool(EquipedTool.Wrench);
        }
    }

    public void EquipTool(EquipedTool newTool)
    {
        fishingConfig.handObject.SetActive(false);
        harpoonConfig.handObject.SetActive(false);
        emptyHandsConfig.handObject.SetActive(false);
        fireExtinguisherConfig.handObject.SetActive(false);
        WrenchConfig.handObject.SetActive(false);

        switch (newTool)
        {
            case EquipedTool.Harpoon:
                harpoonConfig.handObject.SetActive(true);
                break;

            case EquipedTool.EmptyHands:
                emptyHandsConfig.handObject.SetActive(true);
                break;

            case EquipedTool.FireExtinguisher:
                fireExtinguisherConfig.handObject.SetActive(true);
                break;

            case EquipedTool.Wrench:
                WrenchConfig.handObject.SetActive(true);
                break;

            case EquipedTool.Empty:
                break;
        }

        currentTool = newTool;
    }

    public bool IsToolEquipped(EquipedTool tool) => currentTool == tool;
}