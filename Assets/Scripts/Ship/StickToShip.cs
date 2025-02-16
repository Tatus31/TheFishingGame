using UnityEngine;

public class StickToShip : MonoBehaviour
{
    public static StickToShip Instance;

    [Header("Refrences")]
    [SerializeField] Rigidbody shipRb;
    [Header("Mask")]
    [SerializeField] LayerMask shipControlsLayerMask;

    Rigidbody playerRb;

    bool isOnShip;
    public bool IsOnShip { get { return isOnShip; } }
    bool isControlingShip;

    Vector3 localPositionOffset;
    Quaternion localRotationOffset;

    ShipMovement shipMovement;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isOnShip)
            return;

        playerRb.velocity = shipRb.velocity;

        if (isControlingShip)
        {
            transform.localPosition = localPositionOffset;
            transform.localRotation = localRotationOffset;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && MouseWorldPosition.GetInteractable(shipControlsLayerMask) && isOnShip)
        {
            if (isControlingShip)
            {
                isControlingShip = false;
                playerRb.interpolation = RigidbodyInterpolation.Interpolate;

                PlayerMovement.Instance.IsControllable = true;

                if (shipMovement != null)
                {
                    shipMovement.IsControllingShip = false;
                }

                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
                PlayerMovement.Instance.orientation.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }
            else
            {
                localPositionOffset = transform.localPosition;
                localRotationOffset = transform.localRotation;

                isControlingShip = true;
                playerRb.interpolation = RigidbodyInterpolation.None;

                PlayerMovement.Instance.IsControllable = false;

                if (shipMovement != null)
                {
                    shipMovement.IsControllingShip = true;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship"))
        {
            isOnShip = true;
            transform.SetParent(shipRb.transform);

            PlayerMovement.Instance.orientation.SetParent(null, true);

            PlayerMovement.Instance.SwitchState(PlayerMovement.Instance.WalkOnShipState);
            shipMovement = collision.gameObject.GetComponent<ShipMovement>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ship"))
        {
            isOnShip = false;
            transform.SetParent(null);
            PlayerMovement.Instance.orientation.SetParent(transform, true);
        }
    }
}
