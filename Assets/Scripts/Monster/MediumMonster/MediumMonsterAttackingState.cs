using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumMonsterAttackingState : BaseMediumMonsterState
{
    Transform shipTransform;
    Transform playerTransform;
    Transform monsterTransform;

    float swimAttackSpeed = 200f;
    float monsterEscapeTime = 2f;
    float turnSmoothTime = 1.2f;

    int numberOfAttacks = 0;
    int maxNumberOfAttacks = 5;

    Rigidbody rb;

    Vector3 targetDirection;
    Vector3 directionToShip;
    Vector3 currentMoveDirection;

    bool isMonsterRetreating = false;
    bool isPlayerSwimming = false;
    bool shipSank;
    bool isMonsterPursuing = false;

    public MediumMonsterAttackingState(Transform shipTransform, Transform monsterTransform, Transform playerTransform, float swimAttackSpeed, Rigidbody rb, float monsterEscapeTime)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.playerTransform = playerTransform;
        this.swimAttackSpeed = swimAttackSpeed;
        this.rb = rb;
        this.monsterEscapeTime = monsterEscapeTime;
        this.currentMoveDirection = Vector3.zero;
    }

    public override void EnterState(MediumMonsterStateMachine monsterState)
    {
        isMonsterPursuing = true;

        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
        PlayerMovement.Instance.OnPlayerSwimmingChange += PlayerMovement_OnPlayerSwimmingChange;
        SinkShip.OnShipSank += SinkShip_OnShipSank;
        DetectionManager.OnInvestigationEnd += DetectionManager_OnInvestigationEnd;
    }

    private void DetectionManager_OnInvestigationEnd()
    {
        isMonsterPursuing = false;
    }

    private void SinkShip_OnShipSank(bool obj)
    {
        shipSank = obj;
    }

    private void PlayerMovement_OnPlayerSwimmingChange(object sender, bool e)
    {
        isPlayerSwimming = e;
    }

    private void ShipDamage_OnDamageTaken(object sender, int e)
    {
        isMonsterRetreating = true;
        numberOfAttacks++;
    }

    public override void ExitState()
    {
        ShipDamage.Instance.OnDamageTaken -= ShipDamage_OnDamageTaken;
        PlayerMovement.Instance.OnPlayerSwimmingChange -= PlayerMovement_OnPlayerSwimmingChange;
        SinkShip.OnShipSank -= SinkShip_OnShipSank;
        DetectionManager.OnInvestigationEnd -= DetectionManager_OnInvestigationEnd;

        isMonsterPursuing = false;
    }

    public override void UpdateState(MediumMonsterStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            monsterState.LookAt(targetDirection);
        }
    }

    public override void FixedUpdateState(MediumMonsterStateMachine monsterState)
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

        Vector3 targetDirection = isMonsterRetreating ? -directionToShip : directionToShip;
        currentMoveDirection = Vector3.Lerp(currentMoveDirection, targetDirection, Time.fixedDeltaTime / turnSmoothTime);
        rb.AddForce(currentMoveDirection * swimAttackSpeed, ForceMode.Acceleration);

        if (isMonsterRetreating)
        {
            monsterState.StartCoroutine(SwimAwayFromShip(monsterState));
        }
    }

    IEnumerator SwimAwayFromShip(MediumMonsterStateMachine monsterState)
    {
        yield return new WaitForSeconds(monsterEscapeTime);

        if (numberOfAttacks >= maxNumberOfAttacks || shipSank || !isMonsterPursuing)
        {
            numberOfAttacks = 0;
            monsterState.SwitchState(monsterState.IdleState);
        }

        isMonsterRetreating = false;
    }

    public override void DrawGizmos(MediumMonsterStateMachine monsterState)
    {
        Gizmos.color = isMonsterRetreating ? Color.blue : Color.red;
        Gizmos.DrawRay(monsterTransform.position, currentMoveDirection * 5f);
    }
}