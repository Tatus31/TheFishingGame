using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipMovement : MonoBehaviour
{
    public event EventHandler<Vector3> OnShipSpeedChange; 

    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float speedSmoothTime = 0.5f;
    [SerializeField] float rotationSmoothTime = 0.5f;
    //[SerializeField] float deceleration = 5f;
    //[SerializeField] float reverseSpeedChange = 5f;
    //[SerializeField] float maxReverseSpeed = 10f;

    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }

    Rigidbody shipRigidbody;

    float currentSpeed = 0f;
    float currentTurnSpeed = 0f;

    bool isControllingShip;
    public bool IsControllingShip {  get { return isControllingShip; } set { isControllingShip = value; } }

    Vector3 shipFlatVel;
    public Vector3 ShipFlatVel { get { return shipFlatVel; } set { shipFlatVel = value; } }

    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();

        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }

    void FixedUpdate()
    {
        if (!isControllingShip)
            return;

        HandleMovement();
        HandleRotation();

        OnShipSpeedChange?.Invoke(this, ShipFlatVel);
    }

    void HandleMovement()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();

        float moveInput = movementInput.y;
        float targetMoveSpeed = moveInput * maxSpeed;

        float moveSmoothVelocity = 0f;

        //if (moveInput > 0)
        //{
        //    currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
        //}
        //else if (moveInput < 0)
        //{
        //    currentSpeed = Mathf.MoveTowards(currentSpeed, -maxReverseSpeed, reverseSpeedChange * Time.fixedDeltaTime);
        //}
        //else
        //{
        //    currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        //}

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetMoveSpeed, ref moveSmoothVelocity, speedSmoothTime);

        Vector3 moveDirection = transform.forward * currentSpeed;
        shipRigidbody.AddForce(moveDirection, ForceMode.Acceleration);

        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }

    void HandleRotation()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();

        float turnInput = movementInput.x;
        float targetTurnSpeed = turnInput * turnSpeed;

        float rotationSmoothVelocity = 0f;
        currentTurnSpeed = Mathf.SmoothDamp(currentTurnSpeed, targetTurnSpeed, ref rotationSmoothVelocity, rotationSmoothTime);

        Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        shipRigidbody.MoveRotation(shipRigidbody.rotation * turnRotation);
    }
}