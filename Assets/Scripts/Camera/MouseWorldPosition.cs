using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    public static MouseWorldPosition Instance;

    [SerializeField]
    LayerMask groundLayerMask;

    private void Awake()
    {
        Instance = this;
    }

    public static Vector3 GetMouseWorldPosition(float range)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, range, Instance.groundLayerMask);
        return raycastHit.point;
    }
}
