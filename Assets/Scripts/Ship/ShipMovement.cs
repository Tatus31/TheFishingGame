using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 5f;
    [SerializeField] float reverseSpeedChange = 5f;
    [SerializeField] float maxReverseSpeed = 10f;

    Rigidbody shipRigidbody;

    float currentSpeed = 0f;
    float currentTurnSpeed = 0f;

    bool isControllingShip;
    public bool IsControllingShip {  get { return isControllingShip; } set { isControllingShip = value; } }

    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isControllingShip)
            return;

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();

        float moveInput = movementInput.y;

        if (moveInput > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.fixedDeltaTime);
        }
        else if (moveInput < 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, -maxReverseSpeed, reverseSpeedChange * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        Vector3 moveDirection = transform.forward * currentSpeed;
        shipRigidbody.AddForce(moveDirection, ForceMode.Acceleration);
    }

    void HandleRotation()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();

        float turnInput = movementInput.x;
        float targetTurnSpeed = turnInput * turnSpeed;

        float rotationSmoothTime = 0.5f;
        float rotationSmoothVelocity = 0f;
        currentTurnSpeed = Mathf.SmoothDamp(currentTurnSpeed, targetTurnSpeed, ref rotationSmoothVelocity, rotationSmoothTime);

        Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
        shipRigidbody.MoveRotation(shipRigidbody.rotation * turnRotation);
    }
}