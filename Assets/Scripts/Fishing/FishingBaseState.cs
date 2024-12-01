using UnityEngine;

public abstract class FishingBaseState
{
    public abstract void EnterState(FishingStateManager fishingState);
    public abstract void UpdateState(FishingStateManager fishingState);
    public virtual void ExitState() { }
    public virtual void DrawGizmos(FishingStateManager fishingState) { }
}
