using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform orientation;
    [Header("Sensitivity")]
    [SerializeField][Range(0,1)] float sensitivity = 1f;

    public float Sensitivity {  get { return sensitivity; } set {  sensitivity = value; } }

    float sensMultiplier = 5f;

    Vector2 currentMouseDelta;
    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        currentMouseDelta = InputManager.Instance.GetMouseDelta() * sensitivity * Time.fixedDeltaTime * sensMultiplier;
    }

    void LateUpdate()
    {
        MouseLook();
    }

    void MouseLook()
    {
        float desiredX;

        Vector3 rot = cameraHolder.transform.localRotation.eulerAngles;
        desiredX = rot.y + currentMouseDelta.x;

        xRotation -= currentMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, desiredX, cameraHolder.rotation.z);
        PlayerMovement.Instance.orientation.rotation = Quaternion.Euler(0, desiredX, 0);
    }
}
