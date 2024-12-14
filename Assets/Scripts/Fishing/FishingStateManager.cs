using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Cinemachine;

public class FishingStateManager : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] ThrowSettings throwSettings;

    [Header("References")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform fishObject;
    [SerializeField] Slider holdProgressBar;
    [SerializeField] Image pullCheck;

    float cooldownTimer = 0f;

    public bool isCameraLockedOn;

    FishingBaseState currentState;

    AnimationController animator;

    public Throw throwState = new Throw();
    public Catch catchState = new Catch();
    public Reel reelState = new Reel();
    public Flee fleeState = new Flee();
    public Escaped escapedState = new Escaped();

    void Start()
    {
        throwState.Initialize(throwSettings, orientation, fishObject, holdProgressBar);
        catchState.Initialize(pullCheck);
        reelState.Initialize(fishObject, pullCheck, throwSettings.reelInTime);
        fleeState.Initialize(fishObject, pullCheck, throwSettings.reelInTime);

        currentState = throwState;
        currentState.EnterState(this);

        animator = AnimationController.Instance;
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
        currentState.ExitState();
        currentState = newState;
        newState.EnterState(this);
    }
    public bool IsCooldownOver()
    {
        return cooldownTimer <= 0;
    }

    public void StartCooldown()
    {
        cooldownTimer = throwSettings.cooldownTime;
    }

    void OnDrawGizmos()
    {
        currentState?.DrawGizmos(this);
    }

    void OnValidate()
    {
        throwState.Initialize(throwSettings, orientation, fishObject, holdProgressBar);
        catchState.Initialize(pullCheck);
        reelState.Initialize(fishObject, pullCheck, throwSettings.reelInTime);
    }

    public void CameraLock()
    {
        if (!isCameraLockedOn) return;

        Vector3 direction = fishObject.position - virtualCamera.transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        virtualCamera.transform.rotation = Quaternion.Slerp(virtualCamera.transform.rotation, targetRotation, throwSettings.rotationSpeed * Time.deltaTime);
    }

    public Transform GetCurrentTransform() => transform;

    public Vector3 GetStartFishPosition() => reelState.GetStartFishPosition();

    public float GetCurrentReelInTimer() => reelState.GetCurrentReelInTimer();

    public CinemachineVirtualCamera GetCinemachineVirtualCamera() => virtualCamera;

    public Transform GetOrientation() => orientation;

    public AnimationController GetAnimationController() => animator;
}
