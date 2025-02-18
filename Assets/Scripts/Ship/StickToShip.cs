using UnityEngine;

public class StickToShip : MonoBehaviour
{
    public static StickToShip Instance;

    [Header("References")]
    [SerializeField] Rigidbody shipRb;

    [Header("Mask")]
    [SerializeField] LayerMask shipControlsLayerMask;

    [Header("Ship Controls")]
    [SerializeField] float additionalDownForce = 20f;
    [SerializeField] float velocityMatchDuration = 1.5f; 
    [SerializeField] float velocityBlendTime = 0.5f;      

    bool isOnShip;
    public bool IsOnShip { get { return isOnShip; } }

    bool isControllingShip;
    bool isMatchingVelocity = false;

    Vector3 localPositionOffset;
    Quaternion localRotationOffset;

    float velocityMatchEndTime = 0f;
    float velocityBlendEndTime = 0f;

    ShipMovement shipMovement;
    Rigidbody playerRb;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        playerRb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (shipRb == null)
            return;

        if (!isOnShip) return;

        if (isControllingShip)
        {
            transform.localPosition = localPositionOffset;
            transform.localRotation = localRotationOffset;
        }
        else if (isMatchingVelocity)
        {
            if (Time.time < velocityMatchEndTime)
            {
                playerRb.velocity = shipRb.velocity;
            }
            else if (Time.time < velocityBlendEndTime)
            {
                float blendFactor = (Time.time - velocityMatchEndTime) / (velocityBlendEndTime - velocityMatchEndTime);
                Vector3 relativeVelocity = playerRb.velocity - shipRb.velocity;
                playerRb.velocity = Vector3.Lerp(shipRb.velocity, shipRb.velocity + relativeVelocity, blendFactor);
            }
            else
            {
                isMatchingVelocity = false;
            }
        }

        if (!isControllingShip)
        {
            playerRb.AddForce(-transform.up * additionalDownForce, ForceMode.Force);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && MouseWorldPosition.GetInteractable(shipControlsLayerMask) && isOnShip)
        {
            if (isControllingShip)
            {
                isControllingShip = false;

                playerRb.interpolation = RigidbodyInterpolation.Interpolate;
                PlayerMovement.Instance.IsControllable = true;

                if (shipMovement != null)
                {
                    shipMovement.IsControllingShip = false;
                }

                transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
                PlayerMovement.Instance.orientation.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

                velocityMatchEndTime = Time.time + velocityMatchDuration;
                velocityBlendEndTime = velocityMatchEndTime + velocityBlendTime;

                isMatchingVelocity = true;

                playerRb.velocity = shipRb.velocity;
            }
            else
            {
                localPositionOffset = transform.localPosition;
                localRotationOffset = transform.localRotation;

                isControllingShip = true;

                playerRb.interpolation = RigidbodyInterpolation.None;
                PlayerMovement.Instance.IsControllable = false;

                if (shipMovement != null)
                {
                    shipMovement.IsControllingShip = true;
                }

                isMatchingVelocity = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && !isOnShip)
        {
            isOnShip = true;
            transform.SetParent(shipRb.transform);
            PlayerMovement.Instance.orientation.SetParent(null, true);
            shipMovement = collision.gameObject.GetComponent<ShipMovement>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && isOnShip)
        {
            isOnShip = false;
            isControllingShip = false;
            isMatchingVelocity = false;
            transform.SetParent(null);

            PlayerMovement.Instance.orientation.SetParent(transform, true);
            PlayerMovement.Instance.IsControllable = true;

            playerRb.velocity = shipRb.velocity;
        }
    }
}