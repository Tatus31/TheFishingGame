using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLakeMonsterState
{
    public abstract void EnterState(LakeMonsterStateMachine monsterState);
    public abstract void UpdateState(LakeMonsterStateMachine monsterState);
    public abstract void FixedUpdateState(LakeMonsterStateMachine monsterState);
    public abstract void DrawGizmos(LakeMonsterStateMachine monsterState);
    public abstract void ExitState();
    public virtual void ApplyGravityOutsideWater(LakeMonsterStateMachine monsterState)
    {
        if (monsterState.DoubleGravityApplied)
        {
            monsterState.Rb.AddForce(Vector3.down * monsterState.GravityForce, ForceMode.Acceleration);
        }
    }
}
