using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public enum EquipedTool
    {
        Harpoon,
        DivingSuit,
        Empty,
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
    [SerializeField] ToolConfiguration divingSuitConfig;

    EquipedTool currentTool = EquipedTool.Empty;

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
    }

    public void EquipTool(EquipedTool newTool)
    {
        fishingConfig.handObject.SetActive(false);
        harpoonConfig.handObject.SetActive(false);
        divingSuitConfig.handObject.SetActive(false);

        switch (newTool)
        {
            case EquipedTool.Harpoon:
                harpoonConfig.handObject.SetActive(true);
                break;

            case EquipedTool.DivingSuit:
                divingSuitConfig.handObject.SetActive(true);
                break;

            case EquipedTool.Empty:
                break;
        }

        currentTool = newTool;
    }

    public bool IsToolEquipped(EquipedTool tool) => currentTool == tool;
}