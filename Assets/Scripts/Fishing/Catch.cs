using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Catch : FishingBaseState
{
    Image pullCheck;

    float waitTime;
    float pullInWindowDuration = 2.0f;
    bool canPullIn = false;

    public void Initialize(Image pullCheck)
    {
        this.pullCheck = pullCheck;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        pullCheck.color = Color.white;

        fishingState.StartCoroutine(StartCatchSequence(fishingState));
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

    public override void ExitState() { }

    public override void DrawGizmos(FishingStateManager fishingState) { }

    IEnumerator StartCatchSequence(FishingStateManager fishingState)
    {
        waitTime = Random.Range(1f, 5f);
        yield return new WaitForSeconds(waitTime);

        canPullIn = true;

        pullCheck.color = Color.blue;

        yield return new WaitForSeconds(pullInWindowDuration);

        if (canPullIn)
        {
            pullCheck.color = Color.black;

            fishingState.StartCooldown();
            fishingState.SwitchState(fishingState.throwState);
        }
    }
}
