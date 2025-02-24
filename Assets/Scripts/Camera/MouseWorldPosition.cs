using UnityEngine;
using UnityEngine.UI;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance;

    [SerializeField]
    LayerMask groundLayerMask;
    [SerializeField]
    public LayerMask InteractableMask;

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

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, Instance.interactionRange, Instance.InteractableMask);
        return raycastHit.point;
    }

    public static GameObject GetObjectOverMouse(float maxDistance, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance, layerMask))
        {
            return raycastHit.collider.gameObject;
        }
        return null;
    }

    public static GameObject GetObjectOverMouse(LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Instance.interactionRange, layerMask))
        {
            return raycastHit.collider.gameObject;
        }
        return null;
    }

    public static GameObject GetObjectOverMouse(float maxDistance, string tag)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxDistance))
        {
            if (raycastHit.collider.CompareTag(tag))
            {
                return raycastHit.collider.gameObject;
            }
        }
        return null;
    }

    public static GameObject GetObjectOverMouse(string tag)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Instance.interactionRange))
        {
            if (raycastHit.collider.CompareTag(tag))
            {
                return raycastHit.collider.gameObject;
            }
        }
        return null;
    }

    public static bool GetInteractable(LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Instance.interactionRange, Instance.InteractableMask))
        {
            if (((1 << raycastHit.collider.gameObject.layer) & mask) != 0)
            {
                return true;
            }
        }

        return false;
    }
}
