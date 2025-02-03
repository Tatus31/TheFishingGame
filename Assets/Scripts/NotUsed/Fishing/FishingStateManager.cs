using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Cinemachine;

public class FishingStateManager : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] FishingSettings fishingSettings;

    [Header("References")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform fishObject;
    [SerializeField] Slider holdProgressBar;
    [SerializeField] Image pullCheck;

    [Header("LayerMasks")]
    [SerializeField] LayerMask fishingLayerMask;

    float cooldownTimer = 0f;

    public bool isCameraLockedOn;

    FishingBaseState currentState;

    AnimationController animator;

    Animator fishingAnimator;
    Animator lureAnimator;

    public Throw throwState = new Throw();
    public Catch catchState = new Catch();
    public Reel reelState = new Reel();
    //public Flee fleeState = new Flee();
    public Escaped escapedState = new Escaped();

    void Start()
    {
        throwState.Initialize(fishingSettings.minTrajectoryHeight,
        fishingSettings.maxLineLength,
        fishingSettings.minLineLength,
        fishingSettings.lineGrowthRate,
        orientation,
        fishObject,
        holdProgressBar,
        fishingLayerMask);

        catchState.Initialize(pullCheck);

        reelState.Initialize(fishObject,
            pullCheck,
            fishingSettings.reelInTime,
            fishingSettings.minStartFleeingTime,
            fishingSettings.maxStartFleeingTime);

        //fleeState.Initialize(fishObject,
        //    pullCheck,
        //    fishingSettings.reelInTime,
        //    fishingSettings.minFleeTime,
        //    fishingSettings.maxFleeTime,
        //    fishingSettings.fleeRadius,
        //    fishingSettings.fleeTimes);

        currentState = throwState;
        currentState.EnterState(this);

        animator = AnimationController.Instance;

        fishingAnimator = animator.GetAnimator(AnimationController.Animators.FishingAnimator);
        lureAnimator = animator.GetAnimator(AnimationController.Animators.LureAnimator);
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        CameraLock();

        currentState.UpdateState(this);
    }

    public void SwitchState(FishingBaseState newState)
    {
        newState.ExitState();
        currentState = newState;
        newState.EnterState(this);
    }
    public bool IsCooldownOver()
    {
        return cooldownTimer <= 0;
    }

    public void StartCooldown()
    {
        cooldownTimer = fishingSettings.cooldownTime;
    }

    void OnDrawGizmos()
    {
        currentState?.DrawGizmos(this);
    }

    void OnValidate()
    {
        if (fishingSettings == null) return;

        throwState.Initialize(
            fishingSettings.minTrajectoryHeight,
            fishingSettings.maxLineLength,
            fishingSettings.minLineLength,
            fishingSettings.lineGrowthRate,
            orientation,
            fishObject,
            holdProgressBar,
            fishingLayerMask
        );

        if (pullCheck != null)
        {
            catchState.Initialize(pullCheck);
            reelState.Initialize(
                fishObject,
                pullCheck,
                fishingSettings.reelInTime,
                fishingSettings.minStartFleeingTime,
                fishingSettings.maxStartFleeingTime
            );

            //fleeState.Initialize(
            //    fishObject,
            //    pullCheck,
            //    fishingSettings.reelInTime,
            //    fishingSettings.minFleeTime,
            //    fishingSettings.maxFleeTime,
            //    fishingSettings.fleeRadius,
            //    fishingSettings.fleeTimes
            //);
        }
    }

    public void CameraLock()
    {
        if (!isCameraLockedOn) return;

        Vector3 direction = fishObject.position - virtualCamera.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        virtualCamera.transform.rotation = Quaternion.Slerp(virtualCamera.transform.rotation, targetRotation, fishingSettings.rotationSpeed * Time.deltaTime);
    }

    public Transform GetCurrentTransform() => transform;

    public Vector3 GetStartFishPosition() => reelState.GetStartFishPosition();

    public float GetCurrentReelInTimer() => reelState.GetCurrentReelInTimer();

    public CinemachineVirtualCamera GetCinemachineVirtualCamera() => virtualCamera;

    public Transform GetOrientation() => orientation;

    public AnimationController GetAnimationController() => animator;

    public Animator GetFishingAnimator() => fishingAnimator;

    public Animator GetLureAnimator() => lureAnimator;
}
