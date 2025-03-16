using UnityEngine;

public abstract class BaseMonsterState
{
    public abstract void EnterState(MonsterStateMachine monsterState);   
    public abstract void ExitState();
    public abstract void UpdateState(MonsterStateMachine monsterState);
    public abstract void FixedUpdateState(MonsterStateMachine monsterState);
    public abstract void DrawGizmos(MonsterStateMachine monsterState);

    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerExit(Collider other);
}
