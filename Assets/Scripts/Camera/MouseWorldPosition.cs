using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance;

    [SerializeField]
    LayerMask groundLayerMask;
    [SerializeField]
    LayerMask InteractableMask;

    [SerializeField]
    float interactionRange = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.groundLayerMask);
        return raycastHit.point;
    }

    public static bool GetInteractable()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, Instance.interactionRange, Instance.InteractableMask))
        {
            return true; 
        }

        return false; 
    }
}
