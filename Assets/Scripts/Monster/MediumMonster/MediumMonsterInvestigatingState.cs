using UnityEngine;

public class MediumMonsterInvestigatingState : BaseMediumMonsterState
{
    private float investigationSwimSpeed = 8f;
    private float investigationStopDistance = 12f;
    private Transform monsterHead;
    private Rigidbody rb;
    private Transform shipTransform;

    public MediumMonsterInvestigatingState(Transform shipTransform, Transform monsterHead, Rigidbody rb, float investigationSwimSpeed)
    {
        this.shipTransform = shipTransform;
        this.monsterHead = monsterHead;
        this.rb = rb;
        this.investigationSwimSpeed = investigationSwimSpeed;
    }

    public override void EnterState(MediumMonsterStateMachine monster)
    {
        Debug.Log($"Entering Investigating State {monster.transform.name}");

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

        if (distanceToTarget <= investigationStopDistance)
        {
            Debug.Log("Transitioning to next State");
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
        Vector3 labelPosition = monsterHead.position + (investigationPoint - monsterHead.position) * 0.5f;
        string transitionLabel = $"Medium Monster\nDistance to Target: {Vector3.Distance(monsterHead.position, investigationPoint):F2}\n" +
                               $"Threshold: {investigationStopDistance:F2}";
        UnityEditor.Handles.Label(labelPosition, transitionLabel);
#endif
    }
}

