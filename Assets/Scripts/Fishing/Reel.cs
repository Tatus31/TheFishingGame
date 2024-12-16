using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Reel : FishingBaseState
{
    public event EventHandler<bool> OnReeledIn; 

    Transform fishObject;
    Image pullCheck;

    Vector3 startPosition;
    Vector3 targetPosition;

    float reelInTimer;
    float reelInTime;

    bool canReelIn;
    bool reeledIn;

    float fleeTimer;
    float minStartFleeingTime = 2.5f;
    float maxStartFleeingTime = 4.35f;

    public void Initialize(Transform fishObject, Image pullCheck, float reelInTime, float minStartFleeingTime, float maxStartFleeingTime)
    {
        this.fishObject = fishObject;
        this.pullCheck = pullCheck;
        this.reelInTime = reelInTime;
        this.minStartFleeingTime = minStartFleeingTime;
        this.maxStartFleeingTime = maxStartFleeingTime;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        fishingState.GetAnimationController().PlayAnimation(AnimationController.REEL, true);
        StartReeling(fishingState);
        ResetFleeTimer();

        reeledIn = false;
    }

    public override void UpdateState(FishingStateManager fishingState)
    {
        if (!canReelIn)
            return;

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0)
        {
            fishingState.SwitchState(fishingState.fleeState);
            return;
        }

        if (InputManager.Instance.IsLeftMouseButtonHeld())
        {
            float fleeSpeedMultiplier = 0.5f;
            reelInTimer += Time.deltaTime * fleeSpeedMultiplier;
            float t = Mathf.Clamp01(reelInTimer / reelInTime);
            fishObject.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (reelInTimer >= reelInTime)
            {
                reeledIn = true;
                OnReeledIn?.Invoke(this, reeledIn);

                pullCheck.color = Color.white;

                if (reeledIn)
                {
                    //Debug.Log("Reeled in");

                    fishingState.StartCooldown();     
                    
                    //TODO: add a cought state
                    fishingState.SwitchState(fishingState.escapedState);
                }
            }
        }
    }


    void StartReeling(FishingStateManager fishingState)
    {
        pullCheck.color = Color.green;
        canReelIn = true;

        reelInTimer = 0f;

        startPosition = fishObject.position;

        float reelInOffset = 1.1f;
        targetPosition = fishingState.GetCurrentTransform().position + fishingState.GetOrientation().forward * reelInOffset;

        float remainingDistance = Vector3.Distance(fishObject.position, targetPosition);
        float remainingTimeFactor = remainingDistance / fishingState.throwState.GetLineLength();

        reelInTime = Mathf.Lerp(0.5f, 2f, remainingTimeFactor);

        ResetFleeTimer();
    }


    void ResetFleeTimer()
    {
        fleeTimer = Random.Range(minStartFleeingTime, maxStartFleeingTime);
    }

    public Vector3 GetStartFishPosition() => startPosition;

    public float GetCurrentReelInTimer() => reelInTimer;

    public override void DrawGizmos(FishingStateManager fishingState)
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawLine(fishingState.GetCurrentTransform().position + Vector3.up * 0.5f, targetPosition);
    }
}
