using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : BaseMonsterState
{
    Transform shipTransform;
    Transform playerTransform;
    Transform monsterTransform;
    Rigidbody rb;
    ShipMovement shipMovement;

    float swimAttackSpeed = 200f;
    float monsterEscapeTime = 2f;
    float turnSmoothTime = 1.2f;
    float maxAttackDuration = 6f;
    float attackDuration = 0f;

    float predictionValue = 1.5f; 

    int maxNumberOfAttacks = 5;
    int numberOfAttacks = 0;

    Vector3 targetDirection;
    Vector3 directionToShip;
    Vector3 currentMoveDirection;

    bool isMonsterRetreating = false;
    bool isPlayerSwimming = false;
    bool shipSank;
    bool isMonsterPursuing = false;

    public AttackingState(Transform shipTransform, Transform monsterTransform, Transform playerTransform, float swimAttackSpeed, 
        Rigidbody rb, float monsterEscapeTime, float maxAttackDuration, float turnSmoothTime, int maxNumberOfAttacks, float predictionValue)
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
        this.predictionValue = predictionValue;

        if (shipTransform != null)
        {
            shipMovement = shipTransform.GetComponent<ShipMovement>();
        }

        currentMoveDirection = Vector3.zero;
    }

    public override void EnterState(MonsterLargeStateMachine monsterState)
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

    void SetTargetDirection()
    {
        Transform currentTransform;
        Vector3 predictedPosition;

        if (isPlayerSwimming)
        {
            currentTransform = playerTransform;
            predictedPosition = currentTransform.position;
        }
        else
        {
            currentTransform = shipTransform;
            predictedPosition = currentTransform.position;
            
            if (shipMovement != null)
            {
                Vector3 shipVelocity = shipMovement.ShipFlatVel;
                
                float distanceToShip = Vector3.Distance(monsterTransform.position, currentTransform.position);
                float velocityMagnitude = shipVelocity.magnitude;
                
                if (velocityMagnitude > 0.1f)
                {
                    predictedPosition += shipVelocity.normalized * velocityMagnitude * predictionValue;
                }
            }
        }

        directionToShip = (predictedPosition - monsterTransform.position).normalized;
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

    public override void UpdateState(MonsterLargeStateMachine monsterState)
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

    public override void FixedUpdateState(MonsterLargeStateMachine monsterState)
    {
        currentMoveDirection = Vector3.Lerp(currentMoveDirection, targetDirection, Time.fixedDeltaTime / turnSmoothTime);
        rb.AddForce(currentMoveDirection * swimAttackSpeed, ForceMode.Acceleration);

        if (isMonsterRetreating)
        {
            monsterState.StartCoroutine(SwimAwayFromShip(monsterState));
        }
    }

    IEnumerator SwimAwayFromShip(MonsterLargeStateMachine monsterState)
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

    public override void DrawGizmos(MonsterLargeStateMachine monsterState)
    {
        Gizmos.color = isMonsterRetreating ? Color.blue : Color.red;
        Gizmos.DrawRay(monsterTransform.position, currentMoveDirection * 5f);

        Transform currentTarget = isPlayerSwimming ? playerTransform : shipTransform;
        if (currentTarget != null)
        {
            float sphereSize = isMonsterRetreating ? 2f : 1f;
            Gizmos.DrawWireSphere(currentTarget.position, sphereSize);

            if (!isMonsterRetreating && shipMovement != null && !isPlayerSwimming)
            {
                Vector3 shipVelocity = shipMovement.ShipFlatVel;
                if (shipVelocity.magnitude > 0.1f)
                {
                    Vector3 predictedPosition = currentTarget.position + shipVelocity.normalized * predictionValue;

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(predictedPosition, 1.5f);
                    Gizmos.DrawLine(currentTarget.position, predictedPosition);
                }
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }
}