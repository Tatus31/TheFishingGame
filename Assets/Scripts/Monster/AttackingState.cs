using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : BaseMonsterState
{
    Transform shipTransform;
    Transform monsterTransform;

    float swimAttackSpeed = 200f;
    float monsterEscapeTime = 2f;

    Rigidbody rb;

    Vector3 targetDirection;
    Vector3 directionToShip;

    bool isMonsterRetreating = false;

    public AttackingState(Transform shipTransform, Transform monsterTransform, float swimAttackSpeed, Rigidbody rb, float monsterEscapeTime)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.rb = rb;
        this.swimAttackSpeed = swimAttackSpeed;
        this.monsterEscapeTime = monsterEscapeTime;
    }

    public override void EnterState(MonsterStateMachine monsterState)
    {
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
    }

    private void ShipDamage_OnDamageTaken(object sender, int e)
    {
        isMonsterRetreating = true;
    }

    public override void ExitState()
    {
        ShipDamage.Instance.OnDamageTaken -= ShipDamage_OnDamageTaken;
    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            monsterState.LookAt(targetDirection);
        }
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        directionToShip = (shipTransform.position - monsterTransform.position).normalized;

        if (!isMonsterRetreating)
        {
            rb.AddForce(directionToShip * swimAttackSpeed, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(-directionToShip * swimAttackSpeed, ForceMode.Acceleration);
            monsterState.StartCoroutine(SwimAwayFromShip(monsterState));
        }
    }

    IEnumerator SwimAwayFromShip(MonsterStateMachine monsterState)
    {
        yield return new WaitForSeconds(monsterEscapeTime);
        monsterState.SwitchState(monsterState.IdleState);
        isMonsterRetreating = false;
    }

    public override void DrawGizmos(MonsterStateMachine monsterState)
    {
        if (!isMonsterRetreating)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(monsterTransform.position, directionToShip);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}
