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

    float attackDuration = 0f;
    float maxAttackDuration = 6f;

    public MediumMonsterAttackingState(Transform shipTransform, Transform monsterTransform, Transform playerTransform, float swimAttackSpeed, Rigidbody rb, float monsterEscapeTime, float maxAttackDuration, float turnSmoothTime, int maxNumberOfAttacks)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.playerTransform = playerTransform;
        this.swimAttackSpeed = swimAttackSpeed;
        this.rb = rb;
        this.monsterEscapeTime = monsterEscapeTime;
        this.maxAttackDuration = maxAttackDuration;
        this.turnSmoothTime = turnSmoothTime;
        this.maxNumberOfAttacks = maxNumberOfAttacks;

        currentMoveDirection = Vector3.zero;
    }

    public override void EnterState(MediumMonsterStateMachine monsterState)
    {
        isMonsterPursuing = true;
        SetTargetDirection();
        attackDuration = 0f;

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

        SetTargetDirection();
    }

    private void ShipDamage_OnDamageTaken(object sender, int e)
    {
        isMonsterRetreating = true;

        numberOfAttacks++;
        attackDuration = 0f; 

        SetTargetDirection();
    }

    private void SetTargetDirection()
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
        targetDirection = isMonsterRetreating ? -directionToShip : directionToShip;
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
            monsterState.LookAt(rb.velocity.normalized);
        }

        if (!isMonsterRetreating)
        {
            attackDuration += Time.deltaTime;

            if (attackDuration > maxAttackDuration)
            {
                isMonsterRetreating = true;
                attackDuration = 0f;

                SetTargetDirection();
            }
        }
    }

    public override void FixedUpdateState(MediumMonsterStateMachine monsterState)
    {
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
        attackDuration = 0f;

        SetTargetDirection();
    }

    public override void DrawGizmos(MediumMonsterStateMachine monsterState)
    {
        Gizmos.color = isMonsterRetreating ? Color.blue : Color.red;
        Gizmos.DrawRay(monsterTransform.position, currentMoveDirection * 5f);

        Transform currentTarget = isPlayerSwimming ? playerTransform : shipTransform;
        if (currentTarget != null)
        {
            float sphereSize = isMonsterRetreating ? 2f : 1f;
            Gizmos.DrawWireSphere(currentTarget.position, sphereSize);
        }
    }
}