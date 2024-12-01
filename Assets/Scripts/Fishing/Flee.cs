using UnityEngine;
using UnityEngine.UI;

public class Flee : FishingBaseState
{
    Transform fishObject;
    Image pullCheck;

    Vector3 fleeStartPosition;
    Vector3 fleeTargetPosition;

    float reelInTimer = 0;
    float reelInTime = 1f;

    float minFleeTime = 0.5f;
    float maxFleeTime = 2f;
    float fleeTimer;

    public void Initialize(Transform fishObject, Image pullCheck, float reelInTime)
    {
        this.fishObject = fishObject;
        this.pullCheck = pullCheck;
        this.reelInTime = reelInTime;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        pullCheck.color = Color.red;

        fleeStartPosition = fishObject.position;
        Vector3 originalPosition = fishingState.GetStartFishPosition();

        Vector3 offset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        fleeTargetPosition = originalPosition + offset;

        ResetFleeTimer();
        reelInTimer = 0f;
    }

    public override void UpdateState(FishingStateManager fishingState)
    {
        if (reelInTimer < reelInTime)
        {
            reelInTimer += Time.deltaTime;

            float t = reelInTimer / reelInTime;
            t = Mathf.Clamp01(t);

            fishObject.position = Vector3.Lerp(fleeStartPosition, fleeTargetPosition, t);
        }

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0)
        {
            ResetFleeTimer();
            fishingState.SwitchState(fishingState.reelState);
        }
    }

    void ResetFleeTimer()
    {
        fleeTimer = Random.Range(minFleeTime, maxFleeTime);
    }
}
