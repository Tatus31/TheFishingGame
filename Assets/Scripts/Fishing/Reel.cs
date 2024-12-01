using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Reel : FishingBaseState
{
    Transform fishObject;
    Image pullCheck;

    Vector3 startPosition;
    Vector3 targetPosition;

    float reelInTimer = 0f;
    float reelInTime = 2f;

    bool canReelIn;

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

        reelInTimer = 0f;
    }

    public override void UpdateState(FishingStateManager fishingState)
    {
        if (!canReelIn)
            return;

        Debug.Log("reel in progress");

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0)
        {
            Debug.Log("fish is trying to flee");
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
                fishObject.position = targetPosition;

                canReelIn = false;
                Debug.Log("reeled in successfully");

                pullCheck.color = Color.white;

                reelInTimer = 0f;

                fishingState.StartCooldown();
                fishingState.SwitchState(fishingState.throwState);
            }
        }
    }


    void StartReeling(FishingStateManager fishingState)
    {
        pullCheck.color = Color.green;

        canReelIn = true;

        startPosition = fishObject.position;
        targetPosition = fishingState.GetCurrentTransform().position;

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
