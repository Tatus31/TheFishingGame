using UnityEngine;

public class LakeMonsterIdleState : BaseLakeMonsterState
{
    Vector3 currentTarget;
    float timeAtCurrentTarget;
    float idleMovementRadius;
    float swimSpeed;
    float minTimeAtTarget;
    float allowedDistanceFromTarget;
    Rigidbody rb;

    bool hasReachedSafeSpace = false;

    public LakeMonsterIdleState(float idleMovementRadius, float obstacleAvoidanceDistance, float swimSpeed, float minTimeAtTarget, float allowedDistanceFromTarget, Rigidbody rb)
    {
        this.idleMovementRadius = idleMovementRadius;
        this.swimSpeed = swimSpeed;
        this.minTimeAtTarget = minTimeAtTarget;
        this.allowedDistanceFromTarget = allowedDistanceFromTarget;
        this.rb = rb;
    }

    public override void EnterState(LakeMonsterStateMachine monster)
    {
        AudioManager.MuteSound(AudioManager.HeartBeatSound);

        hasReachedSafeSpace = false;
        timeAtCurrentTarget = 0f;

        CameraTransitionManager.Instance.EnableMainCamera();

        currentTarget = monster.GetRandomValidTargetInsideSafeSpace(idleMovementRadius);
    }

    public override void UpdateState(LakeMonsterStateMachine monster)
    {
        StayNearTarget(monster);
    }

    public override void FixedUpdateState(LakeMonsterStateMachine monster)
    {
        SwimToTarget(monster);
    }

    private void StayNearTarget(LakeMonsterStateMachine monster)
    {
        float distanceToTarget = Vector3.Distance(monster.monsterHead.position, currentTarget);

        if (distanceToTarget <= allowedDistanceFromTarget)
        {
            timeAtCurrentTarget += Time.deltaTime;

            if (timeAtCurrentTarget >= minTimeAtTarget)
            {
                if (!hasReachedSafeSpace)
                {
                    hasReachedSafeSpace = true;
                    currentTarget = monster.GetRandomValidTargetInsideSafeSpace(idleMovementRadius);
                }
                else
                {
                    currentTarget = monster.GetRandomValidTarget(monster.monsterHead, idleMovementRadius);
                }

                timeAtCurrentTarget = 0f;
            }
        }
    }

    private void SwimToTarget(LakeMonsterStateMachine monster)
    {
        Vector3 toTarget = (currentTarget - monster.monsterHead.position).normalized;
        Vector3 avoidance = monster.GetSmartAvoidanceDirection();

        Vector3 finalDir = (toTarget + avoidance).normalized;
        if (Vector3.Dot(toTarget, avoidance) < -0.3f)
            finalDir = avoidance.normalized;

        rb.AddForce(finalDir * swimSpeed, ForceMode.Acceleration);
        monster.LookAtTarget(finalDir);
    }

    public override void ExitState()
    {

    }

    public override void DrawGizmos(LakeMonsterStateMachine monster)
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
