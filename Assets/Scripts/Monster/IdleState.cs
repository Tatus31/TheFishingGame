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

    public override void EnterState(MonsterLargeStateMachine monsterState)
    {
        hasTarget = false;
        timeAtTarget = 0f;

        AudioManager.MuteSound(AudioManager.HeartBeatSound);
        AudioManager.MuteSound(AudioManager.HeartBeatSlowSound);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState(MonsterLargeStateMachine monsterState)
    {
        monsterState.LookAtTarget(targetDirection);

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

    public override void FixedUpdateState(MonsterLargeStateMachine monsterState)
    {
        if (!hasTarget)
        {
            currentTarget = monsterState.GetRandomValidTarget(monsterTransform, idleMovementRadius);
            hasTarget = true;
        }

        Vector3 directionToTarget = (currentTarget - monsterTransform.position).normalized;
        Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(obstacleAvoidanceDistance);
        Vector3 combinedDirection = (directionToTarget + obstacleAvoidance).normalized;

        rb.AddForce(combinedDirection * swimSpeed, ForceMode.Acceleration);
    }

    public override void DrawGizmos(MonsterLargeStateMachine monsterState)
    {
        Gizmos.DrawWireSphere(monsterTransform.position, idleMovementRadius);
        Gizmos.DrawSphere(currentTarget, 0.2f);
        Gizmos.DrawLine(monsterTransform.position, monsterTransform.position + monsterTransform.forward * obstacleAvoidanceDistance);
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}
