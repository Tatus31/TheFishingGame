using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMediumMonsterState
{
    public abstract void EnterState(MediumMonsterStateMachine monsterState);
    public abstract void UpdateState(MediumMonsterStateMachine monsterState);
    public abstract void FixedUpdateState(MediumMonsterStateMachine monsterState);
    public abstract void DrawGizmos(MediumMonsterStateMachine monsterState);
    public abstract void ExitState();
}
