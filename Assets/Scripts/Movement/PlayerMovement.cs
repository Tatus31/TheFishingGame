using System;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public event EventHandler<Vector3> OnPlayerSpeedChange;

    [Header("References")]
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float maxSpeed = 5f;
    [SerializeField] float accelAmount = 2f;
    public float maxSpeedTime = 1f;

    [Header("Counter Movement")]
    [SerializeField] float frictionAmount = 2f;

    [Header("Suit Movement")]
    [SerializeField] float suitMaxSpeed = 3.25f;
    [SerializeField] float suitAccelAmount = 1f;

    [Header("Suit Counter Movement")]
    [SerializeField] float suitFrictionAmount = 1f;
    [Space(10)]

    [Header("Sprint Movement")]
    [SerializeField] float sprintMaxSpeed = 8f;
    [SerializeField] float sprintAccelAmount = 4f;

    [Header("Sprint Counter Movement")]
    [SerializeField] float sprintFrictionAmount = 1f;

    [Header("Suit Sprint Movement")]
    [SerializeField] float suitSprintMaxSpeed = 5f;
    [SerializeField] float suitSprintAccelAmount = 2f;

    [Header("Suit Sprint Counter Movement")]
    [SerializeField] float suitSprintFrictionAmount = .5f;

    [Header("Swimming")]
    [SerializeField] float swimMaxSpeed = 5f;
    [SerializeField] float swimAccelAmount = 2f;

    [Header("Water Detection")]
    [SerializeField] private LayerMask waterLayer;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public InputManager inputManager;

    float xDir, yDir;

    bool isSwimming = false;

    AnimationController animator;

    Animator fishingAnimator;
    Animator freeHandAnimator;
    Animator harpoonAnimator;

    public Vector3 FlatVel { get; set; }

    private MovementBaseState currentState;
    public WalkState WalkState { get; private set; }
    public SprintState SprintState { get; private set; }
    public SuitWalkState SuitWalkState { get; private set; }
    public SuitSprintState SuitSprintState { get; private set; }
    public SwimmingState SwimmingState { get; private set; }

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;

        rb = GetComponent<Rigidbody>();

        FlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
        SuitWalkState = new SuitWalkState(this, suitMaxSpeed, suitAccelAmount, suitFrictionAmount);
        SuitSprintState = new SuitSprintState(this, suitSprintMaxSpeed, suitSprintAccelAmount, suitSprintFrictionAmount);
        SwimmingState = new SwimmingState(this, swimMaxSpeed, swimAccelAmount);
    }

    void Start()
    {
        inputManager = InputManager.Instance;
        animator = AnimationController.Instance;

        fishingAnimator = animator.GetAnimator(AnimationController.Animators.FishingAnimator);
        freeHandAnimator = animator.GetAnimator(AnimationController.Animators.EmptyHandsAnimator);
        harpoonAnimator = animator.GetAnimator(AnimationController.Animators.HarpoonAnimator);

        SwitchState(WalkState);
    }

    void OnValidate()
    {
        WalkState = new WalkState(this, maxSpeed, accelAmount, frictionAmount);
        SprintState = new SprintState(this, sprintMaxSpeed, sprintAccelAmount, sprintFrictionAmount);
        SuitWalkState = new SuitWalkState(this, suitMaxSpeed, suitAccelAmount, suitFrictionAmount);
        SuitSprintState = new SuitSprintState(this, suitSprintMaxSpeed, suitSprintAccelAmount, suitSprintFrictionAmount);
    }

    void Update()
    {
        GetPlayerInput();

        if (isSwimming)
        {
            return;
        }

        if (inputManager.IsHoldingSprintKey() && currentState != SprintState)
        {
            SwitchState(SprintState);

            if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.DivingSuit))
            {
                SwitchState(SuitSprintState);
            }
        }
        else if (!inputManager.IsHoldingSprintKey() && currentState != WalkState)
        {
            SwitchState(WalkState);

            if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.DivingSuit))
            {
                SwitchState(SuitWalkState);
            }
        }

        OnPlayerSpeedChange?.Invoke(this, FlatVel);
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

            if(InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.Harpoon))
                animator.PlayAnimation(harpoonAnimator, AnimationController.HARPOON_AIM, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            SwitchState(SuitWalkState);
            isSwimming = false;

            if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.Harpoon))
                animator.PlayAnimation(harpoonAnimator, AnimationController.HARPOON_AIM, false);
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

        return forward * yDir + right * xDir;
    }

    void GetPlayerInput()
    {
        xDir = inputManager.GetPlayerMovement().x;
        yDir = inputManager.GetPlayerMovement().y;
    }

    public AnimationController GetAnimationController() => animator;

    public Animator GetFishingAnimator() => fishingAnimator;

    public Animator GetFreeHandAnimator() => freeHandAnimator;
}
