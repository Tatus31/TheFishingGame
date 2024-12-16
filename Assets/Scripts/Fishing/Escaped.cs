using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escaped : FishingBaseState
{
    public override void EnterState(FishingStateManager fishingState)
    {
        fishingState.isCameraLockedOn = false;
        fishingState.GetAnimationController().PlayAnimation(AnimationController.ON_THROW, false);
        fishingState.GetAnimationController().PlayAnimation(AnimationController.REEL, false);
        fishingState.GetAnimationController().PlayAnimation(AnimationController.DONE_FISHING, true);
        fishingState.SwitchState(fishingState.throwState);
    }

    public override void UpdateState(FishingStateManager fishingState) { }

}
