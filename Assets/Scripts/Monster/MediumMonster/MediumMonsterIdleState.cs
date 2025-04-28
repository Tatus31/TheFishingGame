using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumMonsterIdleState : BaseMediumMonsterState
{
    Vector3 currentTarget;
    float timeAtCurrentTarget;
    float idleMovementRadius;
    float obstacleAvoidanceDistance;
    float swimSpeed;
    float minTimeAtTarget;
    float allowedDistanceFromTarget;
    Rigidbody rb;

    public MediumMonsterIdleState(float idleMovementRadius, float obstacleAvoidanceDistance, float swimSpeed, float minTimeAtTarget, float allowedDistanceFromTarget, Rigidbody rb)
    {
        this.idleMovementRadius = idleMovementRadius;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
        this.swimSpeed = swimSpeed;
        this.minTimeAtTarget = minTimeAtTarget;
        this.allowedDistanceFromTarget = allowedDistanceFromTarget;
        this.rb = rb;
    }

    public override void EnterState(MediumMonsterStateMachine monster)
    {
        currentTarget = monster.GetRandomValidTarget(monster.monsterHead, idleMovementRadius);
        timeAtCurrentTarget = 0f;
    }

    public override void UpdateState(MediumMonsterStateMachine monster)
    {
        float distanceToTarget = Vector3.Distance(monster.monsterHead.position, currentTarget);

        if (distanceToTarget <= allowedDistanceFromTarget)
        {
            timeAtCurrentTarget += Time.deltaTime;

            if (timeAtCurrentTarget >= minTimeAtTarget)
            {
                currentTarget = monster.GetRandomValidTarget(monster.monsterHead, idleMovementRadius);
                timeAtCurrentTarget = 0f;
            }
        }
    }

    public override void FixedUpdateState(MediumMonsterStateMachine monster)
    {
        Vector3 moveDirection = (currentTarget - monster.monsterHead.position).normalized;
        Vector3 avoidanceDirection = monster.GetObstacleAvoidanceDirection(obstacleAvoidanceDistance);

        Vector3 finalDirection = (moveDirection + avoidanceDirection).normalized;
        rb.AddForce(finalDirection * swimSpeed, ForceMode.Acceleration);

        monster.LookAt(finalDirection);
    }

    public override void ExitState()
    {

    }

    public override void DrawGizmos(MediumMonsterStateMachine monster)
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(currentTarget, 0.5f);
        Gizmos.DrawWireSphere(monster.transform.position, idleMovementRadius);

#if UNITY_EDITOR
        Vector3 labelPosition = monster.transform.position + Vector3.up * 2f;
        UnityEditor.Handles.Label(labelPosition, $"Time at target: {timeAtCurrentTarget:F1}/{minTimeAtTarget:F1}");
#endif
    }
}
