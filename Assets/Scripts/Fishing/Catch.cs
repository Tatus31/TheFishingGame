using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Catch : FishingBaseState
{
    Image pullCheck;

    float pullInWindowDuration = 2.0f;

    bool canPullIn;

    float startCatchingTimeMin = 1f;
    float startCatchingTimeMax = 5f;

    public void Initialize(Image pullCheck)
    {
        this.pullCheck = pullCheck;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        fishingState.isCameraLockedOn = true;

        pullCheck.color = Color.white;

        fishingState.StartCoroutine(StartCatching(fishingState));
    }

    public override void UpdateState(FishingStateManager fishingState)
    {
        if (canPullIn && InputManager.Instance.IsLeftMouseButtonPressed())
        {
            canPullIn = false;

            fishingState.StartCooldown();
            fishingState.SwitchState(fishingState.reelState);
        }
    }

    IEnumerator StartCatching(FishingStateManager fishingState)
    {
        float waitTime = Random.Range(startCatchingTimeMin, startCatchingTimeMax);
        yield return new WaitForSeconds(waitTime);

        canPullIn = true;

        pullCheck.color = Color.blue;

        yield return new WaitForSeconds(pullInWindowDuration);

        if (canPullIn)
        {
            pullCheck.color = Color.black;

            fishingState.StartCooldown();
            fishingState.SwitchState(fishingState.escapedState);
        }
    }
}
