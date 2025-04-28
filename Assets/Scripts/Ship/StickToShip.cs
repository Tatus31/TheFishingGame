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

    bool isOnShip;
    public bool IsOnShip { get { return isOnShip; } }

    bool isControllingShip;

    Vector3 localPositionOffset;
    Quaternion localRotationOffset;

    ShipMovement shipMovement;
    Rigidbody playerRb;
    CharacterController characterController;
    Transform shipTransform;
    Vector3 previousShipPosition;
    Quaternion previousShipRotation;
    Vector3 moveDirection;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        playerRb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (shipRb != null)
        {
            shipTransform = shipRb.transform;
            previousShipPosition = shipTransform.position;
            previousShipRotation = shipTransform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (shipRb == null || !isOnShip)
            return;

        if (isControllingShip)
        {
            transform.localPosition = localPositionOffset;
            transform.localRotation = localRotationOffset;
        }
        else
        {
            Vector3 shipMovement = shipTransform.position - previousShipPosition;

            if (characterController != null && characterController.enabled)
            {
                characterController.Move(shipMovement);
            }
            else if (playerRb != null)
            {
                playerRb.MovePosition(playerRb.position + shipMovement);

                Quaternion rotationDelta = shipTransform.rotation * Quaternion.Inverse(previousShipRotation);
                Vector3 playerRelativePosition = transform.position - shipTransform.position;
                Vector3 rotatedPosition = shipTransform.position + rotationDelta * playerRelativePosition;
                Vector3 rotationMovement = rotatedPosition - transform.position;

                playerRb.MovePosition(playerRb.position + rotationMovement);

                playerRb.AddForce(-transform.up * additionalDownForce, ForceMode.Force);
            }
            else
            {
                transform.position += shipMovement;
            }
        }

        previousShipPosition = shipTransform.position;
        previousShipRotation = shipTransform.rotation;
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

            if (playerRb != null)
                //playerRb.interpolation = RigidbodyInterpolation.Interpolate;

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

            isControllingShip = true;

            if (playerRb != null)
                //playerRb.interpolation = RigidbodyInterpolation.None;

            PlayerMovement.Instance.IsControllable = false;

            if (shipMovement != null)
            {
                shipMovement.IsControllingShip = true;
            }
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
            shipTransform = shipRb.transform;

            previousShipPosition = shipTransform.position;
            previousShipRotation = shipTransform.rotation;

            if (playerRb != null && playerRb.isKinematic == false)
            {
                playerRb.velocity = shipRb.GetPointVelocity(transform.position);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ship") && isOnShip)
        {
            isOnShip = false;
            isControllingShip = false;
            transform.SetParent(null);

            PlayerMovement.Instance.orientation.SetParent(transform, true);
            PlayerMovement.Instance.IsControllable = true;

            if (playerRb != null && playerRb.isKinematic == false)
            {
                Vector3 exitVelocity = shipRb.GetPointVelocity(transform.position);
                playerRb.velocity = new Vector3(
                    playerRb.velocity.x + exitVelocity.x * 0.8f,
                    playerRb.velocity.y,
                    playerRb.velocity.z + exitVelocity.z * 0.8f
                );
            }
        }
    }
}