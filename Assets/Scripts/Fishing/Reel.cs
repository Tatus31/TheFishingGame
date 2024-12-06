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

    float reelInTimer = 0f;
    float reelInTime = 2f;

    bool canReelIn;
    bool reeledIn;

    float fleeTimer;
    float minFleeTime = 1f;
    float maxFleeTime = 3f;

    public void Initialize(Transform fishObject, Image pullCheck, float reelInTime)
    {
        this.fishObject = fishObject;
        this.pullCheck = pullCheck;
        this.reelInTime = reelInTime;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        StartReeling(fishingState);
        ResetFleeTimer();

        reeledIn = false;
        reelInTimer = 0f;
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
            reelInTimer += Time.deltaTime;
            float t = Mathf.Clamp01(reelInTimer / reelInTime);
            fishObject.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (reelInTimer >= reelInTime)
            {
                reeledIn = true;
                OnReeledIn?.Invoke(this, reeledIn);

                pullCheck.color = Color.white;

                if (reeledIn)
                {
                    Debug.Log("Reeled in");
                    fishingState.GetCinemachineVirtualCamera().LookAt = null;

                    fishingState.StartCooldown();
                    fishingState.SwitchState(fishingState.throwState);
                }
            }
        }
    }


    void StartReeling(FishingStateManager fishingState)
    {
        pullCheck.color = Color.green;

        canReelIn = true;
        startPosition = fishObject.position;

        if (targetPosition == default || reeledIn)
        {
            targetPosition = fishingState.GetCurrentTransform().position;
        }

        reelInTimer = 0f; 
        ResetFleeTimer();
    }

    void ResetFleeTimer()
    {
        fleeTimer = Random.Range(minFleeTime, maxFleeTime);
    }

    public Vector3 GetStartFishPosition()
    {
        return startPosition;
    }

    public float GetCurrentReelInTimer()
    {
        return reelInTimer;
    }
}
