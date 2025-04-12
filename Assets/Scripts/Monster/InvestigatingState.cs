using UnityEngine;

public class InvestigatingState : BaseMonsterState
{
    Transform shipTransform;
    Transform monsterHead;
    Rigidbody rb;
    float investigationSwimSpeed = 10f;
    float investigationStopDistance = 15f;

    public InvestigatingState(Transform shipTransform, Transform monsterHead, Rigidbody rb, float investigationSwimSpeed)
    {
        this.shipTransform = shipTransform;
        this.monsterHead = monsterHead;
        this.rb = rb;
        this.investigationSwimSpeed = investigationSwimSpeed;
    }

    public override void EnterState(MonsterStateMachine monsterState)
    {
        rb = monsterState.GetComponent<Rigidbody>();
        DetectionManager.Instance.StartInvestigation(shipTransform);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        DetectionManager.Instance.UpdateInvestigationPoint(shipTransform);
        DetectionManager.Instance.DecreaseDetectionTimer();
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();
        Vector3 directionToTarget = (investigationPoint - monsterHead.position).normalized;
        float distanceToTarget = Vector3.Distance(monsterHead.position, investigationPoint);
        Vector3 movement = directionToTarget * investigationSwimSpeed;
        Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(1f);
        movement += obstacleAvoidance;
        rb.AddForce(movement, ForceMode.Acceleration);
        monsterState.LookAtTarget(directionToTarget);

        if (distanceToTarget <= investigationStopDistance)
        {
            monsterState.SwitchState(monsterState.StalkingState);
        }
        else if (!DetectionManager.Instance.ShouldContinueInvestigation())
        {
            DetectionManager.Instance.StartCooldown();
            monsterState.SwitchState(monsterState.IdleState);
            DetectionManager.Instance.IsInvestigating = false;
        }
    }

    public override void DrawGizmos(MonsterStateMachine monsterState)
    {
        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(investigationPoint, 1f);
        Gizmos.DrawLine(monsterHead.position, investigationPoint);
        Vector3 labelPosition = monsterHead.position + (investigationPoint - monsterHead.position) * 0.5f;

        string transitionLabel = $"Transition to Stalking:\nDistance to Target: {Vector3.Distance(monsterHead.position, investigationPoint):F2}\n" + $"Threshold: {investigationStopDistance:F2}";

        //UnityEditor.Handles.Label(labelPosition, transitionLabel);
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}