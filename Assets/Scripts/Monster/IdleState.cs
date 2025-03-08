using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseMonsterState
{
    float idleMovementRadius = 10f;
    float obstacleAvoidanceDistance = 1f;
    float obstacleAvoidanceForce = 3f;
    float swimSpeed = 2f;
    float timeAtTarget = 0f;
    float minTimeAtTarget = 1f;
    float allowedDistanceFromTarget = 0.1f;
    float rotationSpeed = 5f;

    Transform monsterTransform;

    Rigidbody rb;

    LayerMask waterLayer;
    LayerMask obstacleLayer;

    Vector3 targetDirection;
    Vector3 currentTarget;

    bool hasTarget;

    public IdleState(float idleMovementRadius, float obstacleAvoidanceDistance, float obstacleAvoidanceForce,
        float swimSpeed, float minTimeAtTarget, float allowedDistanceFromTarget, float rotationSpeed, LayerMask waterLayer, LayerMask obstacleLayer, Rigidbody rb, Transform monsterHead)
    {
        this.idleMovementRadius = idleMovementRadius;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
        this.obstacleAvoidanceForce = obstacleAvoidanceForce;
        this.swimSpeed = swimSpeed;
        this.minTimeAtTarget = minTimeAtTarget;
        this.allowedDistanceFromTarget = allowedDistanceFromTarget;
        this.rotationSpeed = rotationSpeed;
        this.waterLayer = waterLayer;
        this.obstacleLayer = obstacleLayer;
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
            LookAt(targetDirection);
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

    public override void FixedUpdateState()
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
            Vector3 obstacleAvoidance = GetObstacleAvoidanceDirection();
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

    Vector3 GetObstacleAvoidanceDirection()
    {
        Vector3 avoidanceDirection = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(monsterTransform.position, monsterTransform.forward, out hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            avoidanceDirection = Vector3.Reflect(monsterTransform.forward, hit.normal).normalized;
            avoidanceDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * avoidanceDirection;
        }

        return avoidanceDirection * obstacleAvoidanceForce;
    }

    void LookAt(Vector3 lookAtDirection)
    {
        if (lookAtDirection != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(lookAtDirection);
            monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, targetLookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = false;
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = true;
        }
    }
}
