using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : BaseMonsterState
{
    Transform shipTransform;
    Transform playerTransform;
    Transform monsterTransform;

    float swimAttackSpeed = 200f;
    float monsterEscapeTime = 2f;

    Rigidbody rb;

    Vector3 targetDirection;
    Vector3 directionToShip;

    bool isMonsterRetreating = false;
    bool isPlayerSwimming = false;

    public AttackingState(Transform shipTransform, Transform monsterTransform, Transform playerTransform ,float swimAttackSpeed, Rigidbody rb, float monsterEscapeTime)
    {
        this.shipTransform = shipTransform;
        this.playerTransform = playerTransform;
        this.monsterTransform = monsterTransform;
        this.rb = rb;
        this.swimAttackSpeed = swimAttackSpeed;
        this.monsterEscapeTime = monsterEscapeTime;
    }

    public override void EnterState(MonsterLargeStateMachine monsterState)
    {
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
        PlayerMovement.Instance.OnPlayerSwimmingChange += PlayerMovement_OnPlayerSwimmingChange;
    }

    private void PlayerMovement_OnPlayerSwimmingChange(object sender, bool e)
    {
        isPlayerSwimming = e;
    }

    private void ShipDamage_OnDamageTaken(object sender, int e)
    {
        isMonsterRetreating = true;
    }

    public override void ExitState()
    {
        ShipDamage.Instance.OnDamageTaken -= ShipDamage_OnDamageTaken;
    }

    public override void UpdateState(MonsterLargeStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            monsterState.LookAt(targetDirection);
        }
    }

    public override void FixedUpdateState(MonsterLargeStateMachine monsterState)
    {
        Transform currentTransform;

        if (isPlayerSwimming)
        {
            currentTransform = playerTransform;
        }
        else
        {
            currentTransform = shipTransform;
        }

        directionToShip = (currentTransform.position - monsterTransform.position).normalized;

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

    IEnumerator SwimAwayFromShip(MonsterLargeStateMachine monsterState)
    {
        yield return new WaitForSeconds(monsterEscapeTime);
        monsterState.SwitchState(monsterState.IdleState);
        isMonsterRetreating = false;
    }

    public override void DrawGizmos(MonsterLargeStateMachine monsterState)
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
