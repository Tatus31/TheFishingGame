using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public event EventHandler<Vector3> OnPlayerSpeedChange;
    public event EventHandler<bool> OnPlayerSwimmingChange;

    [Header("References")]
    public Transform orientation;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float accelAmount = 2f;
    public float maxSpeedTime = 1f;
    [SerializeField] float frictionAmount = 2f;
    [Space(10)]

    [Header("Sprint Movement")]
    [SerializeField] float sprintMaxSpeed = 8f;
    [SerializeField] float sprintAccelAmount = 4f;
    [SerializeField] float sprintFrictionAmount = 1f;

    [Header("Swimming")]
    [SerializeField] float swimMaxSpeed = 5f;
    [SerializeField] float swimAccelAmount = 2f;

    [Header("Water Detection")]
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] GameObject volume;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public InputManager inputManager;

    float xDir, yDir;

    bool isSwimming = false;

    public bool isControllable = true;
    public bool IsControllable { get { return isControllable; } set { isControllable = value; } }

    AnimationController animator;
    StickToShip stickToShip;

    Animator fishingAnimator;
    Animator freeHandAnimator;
    Animator harpoonAnimator;

    public Vector3 FlatVel { get; set; }

    private MovementBaseState currentState;
    public MovementBaseState CurrentState { get { return currentState; } set { currentState = value; } }
    public WalkState WalkState { get; private set; }
    public SprintState SprintState { get; private set; }
    public SuitWalkState SuitWalkState { get; private set; }
    public SuitSprintState SuitSprintState { get; private set; }
    public SwimmingState SwimmingState { get; private set; }
    public WalkOnShipState WalkOnShipState { get; private set; }

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        rb = GetComponent<Rigidbody>();

        FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
        SwimmingState = new SwimmingState(this, swimMaxSpeed, swimAccelAmount);
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        animator = AnimationController.Instance;
        stickToShip = GetComponent<StickToShip>();

        fishingAnimator = animator.GetAnimator(AnimationController.Animators.FishingAnimator);
        freeHandAnimator = animator.GetAnimator(AnimationController.Animators.EmptyHandsAnimator);
        harpoonAnimator = animator.GetAnimator(AnimationController.Animators.HarpoonAnimator);

        SwitchState(WalkState);
    }

    void OnValidate()
    {
        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
        SwimmingState = new SwimmingState(this, swimMaxSpeed, swimAccelAmount);
    }

    void Update()
    {
        if (isControllable)
            GetPlayerInput();

        if (isSwimming)
        {
            return;
        }

        if (StickToShip.Instance.IsOnShip)
        {
            return;
        }

        OnPlayerSpeedChange?.Invoke(this, FlatVel);

        currentState.UpdateState(this);
        currentState.UpdateState();
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(MovementBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            SwitchState(SwimmingState);

            isSwimming = true;
            OnPlayerSwimmingChange?.Invoke(this, isSwimming);

            if (volume != null)
                volume.SetActive(false);

            if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.Harpoon))
                animator.PlayAnimation(harpoonAnimator, AnimationController.HARPOON_AIM, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            isSwimming = false;

            SwitchState(WalkState);

            if (volume != null)
                volume.SetActive(true);

            if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.Harpoon))
                animator.PlayAnimation(harpoonAnimator, AnimationController.HARPOON_AIM, false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState != null)
        {
            currentState.EvaluateCollision(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentState != null)
        {
            currentState.EvaluateCollision(collision);
        }
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        return (right * xDir + forward * yDir).normalized;
    }

    void GetPlayerInput()
    {
        xDir = inputManager.GetPlayerMovement().x;
        yDir = inputManager.GetPlayerMovement().y;
    }

    public AnimationController GetAnimationController() => animator;

    public Animator GetFishingAnimator() => fishingAnimator;

    public Animator GetFreeHandAnimator() => freeHandAnimator;

    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            CurrentState.DrawGizmos(this);
        }
    }
}
