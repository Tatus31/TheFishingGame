using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform orientation;
    [Header("Sensitivity")]
    [SerializeField][Range(0, 1)] float sensitivity = 1f;
    [Header("Options")]
    [SerializeField] bool rotateAroundOrentationPoint = true;

    public float Sensitivity { get { return sensitivity; } set { sensitivity = value; } }

    float sensMultiplier = 5f;

    Vector2 currentMouseDelta;
    float xRotation;

    static bool lockCamera;

    public static bool IsCursorForcedVisible = false;

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
        if (IsCursorForcedVisible)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }


        if (!lockCamera)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (lockCamera)
            return;

        if (rotateAroundOrentationPoint)
            MouseAroundOrientationLook();
        else
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

    void MouseAroundOrientationLook()
    {
        float mouseX = currentMouseDelta.x;
        float mouseY = currentMouseDelta.y;

        cameraHolder.RotateAround(orientation.position, Vector3.up, mouseX);

        Vector3 rightAxis = cameraHolder.transform.right;
        cameraHolder.RotateAround(orientation.position, rightAxis, -mouseY);

        cameraHolder.LookAt(orientation.position);
    }

    public static void LockCamera(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            lockCamera = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lockCamera = false;
        }
    }

}
