using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseMonsterState
{
    float idleMovementRadius = 10f;
    float obstacleAvoidanceDistance = 1f;
    float swimSpeed = 2f;
    float timeAtTarget = 0f;
    float minTimeAtTarget = 1f;
    float allowedDistanceFromTarget = 0.1f;

    Transform monsterTransform;

    Rigidbody rb;

    LayerMask waterLayer;

    Vector3 targetDirection;
    Vector3 currentTarget;

    bool hasTarget;

    public IdleState(float idleMovementRadius, float obstacleAvoidanceDistance,
        float swimSpeed, float minTimeAtTarget, float allowedDistanceFromTarget, LayerMask waterLayer, Rigidbody rb, Transform monsterHead)
    {
        this.idleMovementRadius = idleMovementRadius;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
        this.swimSpeed = swimSpeed;
        this.minTimeAtTarget = minTimeAtTarget;
        this.allowedDistanceFromTarget = allowedDistanceFromTarget;
        this.waterLayer = waterLayer;
        this.rb = rb;
        monsterTransform = monsterHead;
    }

    public override void EnterState(MonsterStateMachine monsterState)
    {
    }

    public override void ExitState()
    {

    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            monsterState.LookAt(targetDirection);
        }

        if (hasTarget)
        {
            float distanceToTarget = Vector3.Distance(monsterTransform.position, currentTarget);
            if (distanceToTarget < allowedDistanceFromTarget)
            {
                timeAtTarget += Time.deltaTime;
                if (timeAtTarget >= minTimeAtTarget)
                {
                    hasTarget = false;
                    timeAtTarget = 0f;
                }
            }
        }
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        if (!hasTarget)
        {
            Vector3 newTarget = GetRandomValidTarget();
            if (newTarget != Vector3.zero)
            {
                currentTarget = newTarget;
                hasTarget = true;
            }
        }
        else
        {
            Vector3 directionToTarget = (currentTarget - monsterTransform.position).normalized;
            Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection();
            Vector3 combinedDirection = (directionToTarget + obstacleAvoidance).normalized;

            rb.AddForce(combinedDirection * swimSpeed, ForceMode.Acceleration);
        }
    }

    public override void DrawGizmos(MonsterStateMachine monsterState)
    {
        Gizmos.DrawWireSphere(monsterTransform.position, idleMovementRadius);
        Gizmos.DrawSphere(currentTarget, 0.2f);
        Gizmos.DrawLine(monsterTransform.position, monsterTransform.position + monsterTransform.forward * obstacleAvoidanceDistance);
    }

    Vector3 GetRandomValidTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = monsterTransform.position + Random.insideUnitSphere * idleMovementRadius;

            if (Physics.CheckSphere(randomPoint, 0.1f, waterLayer))
            {
                return randomPoint;
            }
        }

        return monsterTransform.position;
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}
