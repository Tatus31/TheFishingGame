using System.Threading;
using UnityEngine;

public class InvestigatingState : BaseMonsterState
{
    Transform shipTransform;
    Transform monsterHead;
    Rigidbody rb;
    float investigationSwimSpeed = 10f;
    float visionAngle = 45f;
    float visionDistance = 15f;

    public InvestigatingState(Transform shipTransform, Transform monsterHead, Rigidbody rb, float investigationSwimSpeed, float visionAngle, float visionDistance)
    {
        this.shipTransform = shipTransform;
        this.monsterHead = monsterHead;
        this.rb = rb;
        this.investigationSwimSpeed = investigationSwimSpeed;
        this.visionAngle = visionAngle;
        this.visionDistance = visionDistance;
    }

    public override void EnterState(MonsterLargeStateMachine monsterState)
    {
        Debug.Log($"Entering Investigating State {monsterState.transform.name}");

        DetectionManager.Instance.StartInvestigation(monsterHead, shipTransform);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState(MonsterLargeStateMachine monsterState)
    {
        DetectionManager.Instance.UpdateInvestigationPoint(monsterHead, shipTransform);
        DetectionManager.Instance.DecreaseDetectionTimer(monsterHead);
    }

    public override void FixedUpdateState(MonsterLargeStateMachine monsterState)
    {
        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();
        Vector3 directionToTarget = (investigationPoint - monsterHead.position).normalized;
        float distanceToTarget = Vector3.Distance(monsterHead.position, investigationPoint);

        Vector3 movement = directionToTarget * investigationSwimSpeed;
        Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(1f);
        movement += obstacleAvoidance;

        rb.AddForce(movement, ForceMode.Acceleration);
        monsterState.LookAtTarget(directionToTarget);

        if (monsterState.IsInConeOfVision(monsterHead, investigationPoint))
        {
            monsterState.SwitchState(monsterState.AttackingState);
        }
        else if (!DetectionManager.Instance.ShouldContinueInvestigation(monsterHead))
        {
            monsterState.SwitchState(monsterState.IdleState);
        }
    }

    public override void DrawGizmos(MonsterLargeStateMachine monsterState)
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

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}