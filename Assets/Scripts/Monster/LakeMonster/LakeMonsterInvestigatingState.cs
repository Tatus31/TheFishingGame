using System.Threading;
using UnityEngine;

public class LakeMonsterInvestigatingState : BaseLakeMonsterState
{
    float investigationSwimSpeed = 8f;
    float visionAngle = 45f;
    float visionDistance = 15f;
    Transform monsterHead;
    Rigidbody rb;
    Transform shipTransform;
    float monsterDetectionTreshold = 3f;

    public LakeMonsterInvestigatingState(Transform shipTransform, Transform monsterHead, Rigidbody rb, float investigationSwimSpeed, float visionAngle, float visionDistance)
    {
        this.shipTransform = shipTransform;
        this.monsterHead = monsterHead;
        this.rb = rb;
        this.investigationSwimSpeed = investigationSwimSpeed;
        this.visionAngle = visionAngle;
        this.visionDistance = visionDistance;
    }

    public override void EnterState(LakeMonsterStateMachine monster)
    {
        AudioManager.PlaySound(AudioManager.HeartBeatSlowSound);

#if UNITY_EDITOR
        Debug.Log($"Entering Investigating State {monster.transform.name}");
#endif
        DetectionManager.Instance.StartInvestigation(monsterHead, shipTransform);
    }

    public override void UpdateState(LakeMonsterStateMachine monster)
    {
        DetectionManager.Instance.UpdateInvestigationPoint(monsterHead, shipTransform);
        DetectionManager.Instance.DecreaseDetectionTimer(monsterHead);
    }

    public override void FixedUpdateState(LakeMonsterStateMachine monster)
    {
        Vector3 investigationPoint = DetectionManager.Instance.GetInvestigationPoint();

        MonsterNavigateToPoint(monster, investigationPoint);

        if (monster.IsInConeOfVision(monsterHead, investigationPoint))
        {
            if(DetectionManager.Instance.GetCurrentDetectionValue() >= monsterDetectionTreshold)
            {
                Debug.Log($"Switching to Attacking State from Investigating State with {DetectionManager.Instance.GetCurrentDetectionValue()} detection value");
                monster.SwitchState(monster.AttackingState);
            }
            else
            {
                monster.SwitchState(monster.IdleState);
            }

        }
        else if (!DetectionManager.Instance.ShouldContinueInvestigation(monsterHead))
        {
            monster.SwitchState(monster.IdleState);
        }

        ApplyGravityOutsideWater(monster);
    }

    private void MonsterNavigateToPoint(LakeMonsterStateMachine monster, Vector3 investigationPoint)
    {
        Vector3 toTarget = (investigationPoint - monsterHead.position).normalized;
        Vector3 avoidance = monster.GetSmartAvoidanceDirection();

        Vector3 finalDir = (toTarget + avoidance).normalized;

        if (Vector3.Dot(toTarget, avoidance) < -0.3f)
            finalDir = avoidance.normalized;

        rb.AddForce(finalDir * investigationSwimSpeed, ForceMode.Acceleration);
        monster.LookAtTarget(finalDir);
    }

    public override void ExitState()
    {
    }

    public override void DrawGizmos(LakeMonsterStateMachine monsterState)
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
