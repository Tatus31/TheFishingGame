using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipMovement : MonoBehaviour
{
    public event EventHandler<Vector3> OnShipSpeedChange;

    [Header("Movement Settings")]
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float speedSmoothTime = 0.5f;
    [SerializeField] float rotationSmoothTime = 0.5f;

    [Header("Water Physics Settings")]
    [SerializeField] float waterDeceleration = 0.2f;
    [SerializeField] float waterDragMultiplier = 1.2f;

    Rigidbody shipRigidbody;
    StableFloatingRigidBody buoyancySystem;

    float currentSpeed = 0f;
    float currentTurnSpeed = 0f;
    float speedSmoothVelocity = 0f;
    float rotationSmoothVelocity = 0f;

    bool isControllingShip;

    Vector3 shipFlatVel;

    public Vector3 ShipFlatVel { get { return shipFlatVel; } set { shipFlatVel = value; } }
    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
    public bool IsControllingShip { get { return isControllingShip; } set { isControllingShip = value; } }

    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();

        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }

    private void Start()
    {
        if (buoyancySystem != null)
        {
            buoyancySystem.Buoyancy = shipRigidbody.mass * 1.1f;
        }
    }

    void FixedUpdate()
    {
        if (!isControllingShip)
            return;

        HandleMovement();
        HandleRotation();
        UpdateShipState();

        OnShipSpeedChange?.Invoke(this, ShipFlatVel);
    }

    void HandleMovement()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();
        float moveInput = movementInput.y;
        float targetMoveSpeed = moveInput * maxSpeed;

        if (Mathf.Abs(moveInput) > 0.01f)
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetMoveSpeed, ref speedSmoothVelocity, speedSmoothTime);
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, waterDeceleration * Time.fixedDeltaTime);

        Vector3 moveDirection = transform.forward * currentSpeed;
        shipRigidbody.AddForce(moveDirection, ForceMode.Acceleration);

        float speedSquared = shipRigidbody.velocity.sqrMagnitude;
        Vector3 waterResistance = -shipRigidbody.velocity.normalized * speedSquared * waterDragMultiplier * 0.01f;
        shipRigidbody.AddForce(waterResistance, ForceMode.Acceleration);
    }

    void HandleRotation()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();
        float turnInput = movementInput.x;
        float targetTurnSpeed = turnInput * turnSpeed;

        currentTurnSpeed = Mathf.SmoothDamp(currentTurnSpeed, targetTurnSpeed, ref rotationSmoothVelocity, rotationSmoothTime);

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
            shipRigidbody.MoveRotation(shipRigidbody.rotation * turnRotation);
        }
    }

    void UpdateShipState()
    {
        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }
}