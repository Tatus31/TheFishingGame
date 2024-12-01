using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

public class FishingStateManager : MonoBehaviour
{
    [Header("Serialized Objects")]
    [SerializeField] ThrowSettings throwSettings;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform hitVisual;
    [SerializeField] Slider holdProgressBar;
    [SerializeField] Image pullCheck;

    float reelInTime = 2f;

    float cooldownTime = 2f;
    float cooldownTimer = 0f;

    FishingBaseState currentState;

    public Throw throwState = new Throw();
    public Catch catchState = new Catch();
    public Reel reelState = new Reel();
    public Flee fleeState = new Flee();

    void Start()
    {
        throwState.Initialize(throwSettings, orientation, hitVisual, holdProgressBar);
        catchState.Initialize(pullCheck);
        reelState.Initialize(hitVisual, pullCheck, reelInTime);
        fleeState.Initialize(hitVisual, pullCheck, reelInTime);

        currentState = throwState;
        currentState.EnterState(this);
    }

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

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
        cooldownTimer = cooldownTime;

    }

    void OnDrawGizmos()
    {
        currentState?.DrawGizmos(this);
    }

    void OnValidate()
    {
        throwState.Initialize(throwSettings, orientation, hitVisual, holdProgressBar);
        catchState.Initialize(pullCheck);
        reelState.Initialize(hitVisual, pullCheck, reelInTime);
    }

    public Transform GetCurrentTransform() => transform;

    public Vector3 GetStartFishPosition() => reelState.GetStartFishPosition();

    public float GetCurrentReelInTimer() => reelState.GetCurrentReelInTimer();
}
