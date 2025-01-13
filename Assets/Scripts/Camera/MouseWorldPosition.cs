using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance;

    [SerializeField]
    LayerMask groundLayerMask;
    [SerializeField]
    public LayerMask InteractableMask;
    [SerializeField]
    LayerMask HarpoonInteractableMask;

    [SerializeField]
    float interactionRange = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public static Vector3 GetMouseWorldPosition(float maxDistance, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, layerMask);
        return raycastHit.point;
    }

    public static bool GetInteractable(LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Instance.interactionRange, Instance.InteractableMask))
        {
            if (((1 << hitInfo.collider.gameObject.layer) & mask) != 0)
            {
                return true;
            }
        }

        return false;
    }
}
