using UnityEngine;

public abstract class BaseMonsterState
{
    public abstract void EnterState(MonsterLargeStateMachine monsterState);
    public abstract void ExitState();
    public abstract void UpdateState(MonsterLargeStateMachine monsterState);
    public abstract void FixedUpdateState(MonsterLargeStateMachine monsterState);
    public abstract void DrawGizmos(MonsterLargeStateMachine monsterState);
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
