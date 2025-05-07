using UnityEngine;

public class MediumMonsterInvestigatingState : BaseMediumMonsterState
{
    float investigationSwimSpeed = 8f;
    float visionAngle = 45f;
    float visionDistance = 15f;
    Transform monsterHead;
    Rigidbody rb;
    Transform shipTransform;

    public MediumMonsterInvestigatingState(Transform shipTransform, Transform monsterHead, Rigidbody rb, float investigationSwimSpeed, float visionAngle, float visionDistance)
    {
        this.shipTransform = shipTransform;
        this.monsterHead = monsterHead;
        this.rb = rb;
        this.investigationSwimSpeed = investigationSwimSpeed;
        this.visionAngle = visionAngle;
        this.visionDistance = visionDistance;
    }

    public override void EnterState(MediumMonsterStateMachine monster)
    {
        AudioManager.PlaySound(AudioManager.HeartBeatSlowSound);

#if UNITY_EDITOR
        Debug.Log($"Entering Investigating State {monster.transform.name}");
#endif
        DetectionManager.Instance.StartInvestigation(monsterHead, shipTransform);
    }

    public override void UpdateState(MediumMonsterStateMachine monster)
    {
        DetectionManager.Instance.UpdateInvestigationPoint(monsterHead, shipTransform);
        DetectionManager.Instance.DecreaseDetectionTimer(monsterHead);
    }

    public override void FixedUpdateState(MediumMonsterStateMachine monster)
    {
        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();
        Vector3 directionToTarget = (investigationPoint - monsterHead.position).normalized;
        float distanceToTarget = Vector3.Distance(monsterHead.position, investigationPoint);

        Vector3 movement = directionToTarget * investigationSwimSpeed;
        Vector3 obstacleAvoidance = monster.GetObstacleAvoidanceDirection(monster.obstacleAvoidanceDistance);
        movement += obstacleAvoidance;

        rb.AddForce(movement, ForceMode.Acceleration);
        monster.LookAtTarget(directionToTarget);

        if (monster.IsInConeOfVision(monsterHead, investigationPoint))
        {
            monster.SwitchState(monster.AttackingState);
        }
        else if (!DetectionManager.Instance.ShouldContinueInvestigation(monsterHead))
        {
            monster.SwitchState(monster.IdleState);
        }
    }

    public override void ExitState()
    {
    }

    public override void DrawGizmos(MediumMonsterStateMachine monsterState)
    {
        if (monsterHead == null) return;

        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(investigationPoint, 1f);
        Gizmos.DrawLine(monsterHead.position, investigationPoint);

#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.2f);
        UnityEditor.Handles.DrawSolidArc(
            monsterHead.position,
            Vector3.up, 
            Quaternion.Euler(0, -visionAngle, 0) * monsterHead.forward,
            visionAngle * 2f,
            visionDistance
        );

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireArc(
            monsterHead.position,
            Vector3.up,
            Quaternion.Euler(0, -visionAngle, 0) * monsterHead.forward,
            visionAngle * 2f,
            visionDistance
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(monsterHead.position, monsterHead.forward * visionDistance);
#endif
    }

}
