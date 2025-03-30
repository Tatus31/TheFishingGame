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
    [SerializeField] float platformFriction = 0.9f; 
    [SerializeField] float platformVelocityTransfer = 0.8f; 

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
    Vector3 lastShipVelocity;
    Vector3 shipAcceleration;
    Vector3 lastShipPosition;
    Quaternion lastShipRotation;
    Vector3 shipLocalPoint;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (shipRb != null)
        {
            lastShipVelocity = shipRb.velocity;
            lastShipPosition = shipRb.position;
            lastShipRotation = shipRb.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (shipRb == null)
            return;

        shipAcceleration = (shipRb.velocity - lastShipVelocity) / Time.fixedDeltaTime;
        lastShipVelocity = shipRb.velocity;

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
        else if (isOnShip && !isControllingShip)
        {

            Vector3 shipPointVelocity = shipRb.GetPointVelocity(transform.position);
            Vector3 relativeVelocity = playerRb.velocity - shipPointVelocity;

            relativeVelocity *= platformFriction;

            Vector3 inertialForce = -shipAcceleration * playerRb.mass * 0.2f;

            playerRb.velocity = shipPointVelocity * platformVelocityTransfer + relativeVelocity;
            playerRb.AddForce(inertialForce, ForceMode.Force);

            if (shipRb.angularVelocity.magnitude > 0.01f)
            {
                Vector3 relativePos = transform.position - shipRb.position;
                Vector3 tangentialVelocity = Vector3.Cross(shipRb.angularVelocity, relativePos);
                playerRb.velocity += tangentialVelocity * platformVelocityTransfer * 0.5f;
            }
        }

        if (!isControllingShip && isOnShip)
        {
            playerRb.AddForce(-transform.up * additionalDownForce, ForceMode.Force);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && MouseWorldPosition.GetInteractable(shipControlsLayerMask) && isOnShip)
        {
            ToggleShipControl();
        }
    }

    private void ToggleShipControl()
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && !isOnShip && !ChangeWaterLevelUnderDeck.Instance.ShipSank)
        {
            isOnShip = true;
            transform.SetParent(shipRb.transform);
            PlayerMovement.Instance.orientation.SetParent(null, true);
            shipMovement = collision.gameObject.GetComponent<ShipMovement>();

            shipLocalPoint = shipRb.transform.InverseTransformPoint(transform.position);
            playerRb.velocity = shipRb.GetPointVelocity(transform.position);
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

            Vector3 exitVelocity = shipRb.GetPointVelocity(transform.position);
            playerRb.velocity = new Vector3(
                playerRb.velocity.x + exitVelocity.x * 0.8f,
                playerRb.velocity.y,
                playerRb.velocity.z + exitVelocity.z * 0.8f
            );
        }
    }
}