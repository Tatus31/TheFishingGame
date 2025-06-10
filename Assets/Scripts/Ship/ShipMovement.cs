using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class ShipMovement : MonoBehaviour
{
    public event EventHandler<Vector3> OnShipSpeedChange;
    public static event EventHandler<SpeedLevel> OnDetectionChange;

    [Serializable]
    public enum SpeedLevel
    {
        reverse,
        neutral,
        forward1,
        forward2,
        forward3
    }

    [Header("Movement Settings")]
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float speedSmoothTime = 0.5f;
    [SerializeField] float rotationSmoothTime = 0.5f;
    [Header("Movement Speed")]
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float forward1Speed = 1.5f;
    [SerializeField] float forward2Speed = 2f;
    [SerializeField] float forward3Speed = 3f;

    [Header("Steering Wheel Settings")]
    [SerializeField] float maxWheelRotation = 90f;
    [SerializeField] float wheelReturnSpeed = 5f;

    [Header("Water Physics Settings")]
    [SerializeField] float waterDeceleration = 0.2f;
    [SerializeField] float waterDragMultiplier = 1.2f;

    [SerializeField] SpeedLevel currentSpeedLevel;

    Rigidbody shipRigidbody;
    StableFloatingRigidBody buoyancySystem;

    float currentSpeed = 0f;
    float currentTurnSpeed = 0f;
    float speedSmoothVelocity = 0f;
    float rotationSmoothVelocity = 0f;
    float currentWheelRotation = 0f;

    bool isControllingShip;

    Vector3 shipFlatVel;

    public Vector3 ShipFlatVel { get { return shipFlatVel; } set { shipFlatVel = value; } }
    public float MaxSpeed { get { return maxSpeed; } set { maxSpeed = value; } }
    public bool IsControllingShip { get { return isControllingShip; } set { isControllingShip = value; } }
    public float CurrentWheelRotation { get { return currentWheelRotation; } }
    public SpeedLevel CurrentSpeedLevel { get { return currentSpeedLevel; } }

    void Awake()
    {
        shipRigidbody = GetComponent<Rigidbody>();
        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }

    private void Start()
    {
        currentSpeedLevel = SpeedLevel.neutral;
        if (buoyancySystem != null)
        {
            buoyancySystem.Buoyancy = shipRigidbody.mass * 1.1f;
        }

        UpdateGearAnimation();
    }

    private void Update()
    {
        if (isControllingShip)
        {
            HandleSpeedLevelInput();
        }
    }

    void HandleSpeedLevelInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            IncreaseSpeedLevel();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DecreaseSpeedLevel();
        }
    }

    public void IncreaseSpeedLevel()
    {
        SpeedLevel previousLevel = currentSpeedLevel;

        switch (currentSpeedLevel)
        {
            case SpeedLevel.reverse:
                currentSpeedLevel = SpeedLevel.neutral;
                break;
            case SpeedLevel.neutral:
                currentSpeedLevel = SpeedLevel.forward1;
                break;
            case SpeedLevel.forward1:
                currentSpeedLevel = SpeedLevel.forward2;
                break;
            case SpeedLevel.forward2:
                currentSpeedLevel = SpeedLevel.forward3;
                break;
            case SpeedLevel.forward3:
                break;
        }

        if (previousLevel != currentSpeedLevel)
        {
            UpdateGearAnimation();
        }

        OnDetectionChange?.Invoke(this, currentSpeedLevel);
    }

    public void DecreaseSpeedLevel()
    {
        SpeedLevel previousLevel = currentSpeedLevel;

        switch (currentSpeedLevel)
        {
            case SpeedLevel.forward3:
                currentSpeedLevel = SpeedLevel.forward2;
                break;
            case SpeedLevel.forward2:
                currentSpeedLevel = SpeedLevel.forward1;
                break;
            case SpeedLevel.forward1:
                currentSpeedLevel = SpeedLevel.neutral;
                break;
            case SpeedLevel.neutral:
                currentSpeedLevel = SpeedLevel.reverse;
                break;
            case SpeedLevel.reverse:
                break;
        }

        if (previousLevel != currentSpeedLevel)
        {
            UpdateGearAnimation();
        }

        OnDetectionChange?.Invoke(this, currentSpeedLevel);
    }

    void UpdateGearAnimation()
    {
        if (AnimationController.Instance == null)
            return;

        Animator gearAnimator = AnimationController.Instance.GetAnimator(AnimationController.Animators.ShipGearAnimator);

        if (gearAnimator == null)
            return;

        int gearState = (int)currentSpeedLevel;
        AnimationController.Instance.PlayAnimation(gearAnimator, "gearState", gearState);
    } 

    void FixedUpdate()
    {
        //if (!isControllingShip)
        //    return;

        HandleMovement();
        HandleRotation();
        UpdateShipState();

        OnShipSpeedChange?.Invoke(this, ShipFlatVel);
    }

    void HandleMovement()
    {
        float moveInput = 0;
        switch (currentSpeedLevel)
        {
            case SpeedLevel.neutral:
                moveInput = 0;
                break;
            case SpeedLevel.forward1:
                moveInput = 1f;
                maxSpeed = forward1Speed;
                break;
            case SpeedLevel.forward2:
                moveInput = 1f;
                maxSpeed = forward2Speed;
                break;
            case SpeedLevel.forward3:
                moveInput = 1f;
                maxSpeed = forward3Speed;
                break;
            case SpeedLevel.reverse:
                moveInput = -1f;
                break;
        }

        float targetMoveSpeed = moveInput * maxSpeed;

        if (moveInput != 0)
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

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float targetWheelRotation = turnInput * maxWheelRotation;
            currentWheelRotation = Mathf.Lerp(currentWheelRotation, targetWheelRotation, Time.fixedDeltaTime * wheelReturnSpeed);
        }
        else
            currentWheelRotation = Mathf.Lerp(currentWheelRotation, 0f, Time.fixedDeltaTime * wheelReturnSpeed);

        currentWheelRotation = Mathf.Clamp(currentWheelRotation, -maxWheelRotation, maxWheelRotation);

        float wheelRotationPercentage = Mathf.Abs(currentWheelRotation) / maxWheelRotation;
        float targetTurnSpeed = Mathf.Sign(currentWheelRotation) * turnSpeed * wheelRotationPercentage;

        currentTurnSpeed = Mathf.SmoothDamp(currentTurnSpeed, targetTurnSpeed, ref rotationSmoothVelocity, rotationSmoothTime);

        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, currentTurnSpeed * Time.fixedDeltaTime, 0f);
            shipRigidbody.MoveRotation(shipRigidbody.rotation * turnRotation);
        }

        //Debug.Log(MathF.Floor(currentWheelRotation));
    }

    void UpdateShipState()
    {
        ShipFlatVel = new Vector3(shipRigidbody.velocity.x, 0f, shipRigidbody.velocity.z);
    }

    public void SetNeutralSpeed()
    {
        SpeedLevel previousLevel = currentSpeedLevel;
        currentSpeedLevel = SpeedLevel.neutral;

        if (previousLevel != currentSpeedLevel)
        {
            UpdateGearAnimation();
        }
    }
}