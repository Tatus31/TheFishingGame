using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public event EventHandler<Vector3> OnPlayerSpeedChange;

    [Header("References")]
    [SerializeField] Transform orientation;
    //TODO: Make script for all the item interactions and shit
    [SerializeField] GameObject fishingHands;
    [SerializeField] GameObject divingSuitHands;
    [SerializeField] FishingStateManager fishingStateManager;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float accelAmount = 2f;
    public float maxSpeedTime = 1f;

    [Header("Counter Movement")]
    [SerializeField] float frictionAmount = 2f;
    [Space(10)]

    [Header("Sprint Movement")]
    [SerializeField] float sprintMaxSpeed = 8f;
    [SerializeField] float sprintAccelAmount = 4f;

    [Header("Sprint Counter Movement")]
    [SerializeField] float sprintFrictionAmount = 1f;

    [HideInInspector] public Rigidbody rb;
    InputManager inputManager;

    float xDir, yDir;

    public bool isInDivingSuit = false;

    AnimationController animator;

    Animator fishingAnimator;
    Animator suitAnimator;

    public Vector3 FlatVel { get; set; }

    private IMovementState currentState;
    public WalkState WalkState { get; private set; }
    public SprintState SprintState { get; private set; }
    public WalkState SuitWalkState { get; private set; }
    public SprintState SuitSprintState { get; private set; }

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        rb = GetComponent<Rigidbody>();

        FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        animator = AnimationController.Instance;
        fishingAnimator = animator.GetAnimator(AnimationController.Animators.FishingAnimator);
        suitAnimator = animator.GetAnimator(AnimationController.Animators.DivingSuitAnimator);

        SwitchState(WalkState);
    }

    void OnValidate()
    {
        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
    }

    void Update()
    {
        GetPlayerInput();

        if (MouseWorldPosition.GetInteractable() && !isInDivingSuit && Input.GetKeyDown(KeyCode.E))
        {
            isInDivingSuit = true;
            divingSuitHands.SetActive(true);
            fishingHands.SetActive(false);
            fishingStateManager.enabled = false;
        }
        else if (MouseWorldPosition.GetInteractable() && isInDivingSuit && Input.GetKeyDown(KeyCode.E))
        {
            isInDivingSuit = false;
            divingSuitHands.SetActive(false);
            fishingHands.SetActive(true);
            fishingStateManager.enabled = true;
        }

        if (inputManager.IsHoldingSprintKey() && currentState != SprintState)
        {
            SwitchState(SprintState);
        }
        else if (!inputManager.IsHoldingSprintKey() && currentState != WalkState)
        {
            SwitchState(WalkState);
        }

        OnPlayerSpeedChange?.Invoke(this, FlatVel);
        currentState.UpdateState();
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(IMovementState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return forward * yDir + right * xDir;
    }

    void GetPlayerInput()
    {
        xDir = inputManager.GetPlayerMovement().x;
        yDir = inputManager.GetPlayerMovement().y;
    }

    public AnimationController GetAnimationController() => animator;

    public Animator GetFishingAnimator() => fishingAnimator;
}
