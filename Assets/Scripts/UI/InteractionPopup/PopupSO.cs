using UnityEngine;

[CreateAssetMenu(fileName = "Popup", menuName = "Popups/Create new Popup", order = 1)]
public class PopupSO : ScriptableObject
{
    public KeyCode InteractionKey;
    public KeyCode InformationKey;
    public LayerMask InteractableLayer;
    public bool IsInteractionActive;
    [TextArea(15, 20)] public string InfoText;
}
