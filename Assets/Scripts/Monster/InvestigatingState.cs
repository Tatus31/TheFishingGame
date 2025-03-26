using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatingState : BaseMonsterState
{
    Transform shipTransform;
    Transform monsterTransform;

    float detectShipValue = 10f;
    float detectionMultiplier = 5f;
    float baseInvestigationRadius = 50f;
    float investigationRadius = 50f;
    float swimSpeed = 2f;
    float obstacleAvoidanceDistance = 1f;

    Vector3 currentTarget;
    Vector3 targetDirection;
    Transform decoyPosition;

    bool hasTarget;
    bool isDecoyActive;

    Rigidbody rb;

    float investigationTimer = 0f;
    float maxInvestigationTime = 30f;

    public InvestigatingState(Transform shipTransform, Transform monsterTransform, float investigationRadius, float swimSpeed, float obstacleAvoidanceDistance, Rigidbody rb)
    {
        baseInvestigationRadius = investigationRadius;
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.investigationRadius = investigationRadius;
        this.swimSpeed = swimSpeed;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
        this.rb = rb;
    }

    public override void EnterState(MonsterStateMachine monsterState)
    {
        //Debug.Log($"Investigating in a radius of {investigationRadius}");
        hasTarget = false;

        if (monsterState.PreviousState == monsterState.IdleState)
        {
            investigationTimer = 0f;
        }
        DetectionManager.OnDetectionValueChange += DetectionManager_OnDetectionValueChange;

        AudioManager.PlaySound(AudioManager.HeartBeatSound);
    }

    private void DetectionManager_OnDetectionValueChange(object sender, float e)
    {
        SetInvestigationRadius(e);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        monsterState.LookAtTarget(targetDirection);

        investigationTimer += Time.deltaTime;

        bool timeoutOccurred = investigationTimer >= maxInvestigationTime;

        if (timeoutOccurred)
        {
            monsterState.SwitchState(monsterState.IdleState);
            DetectionManager.Instance.IsInvestigating = false;
            return;
        }

        if (hasTarget)
        {
            float distanceToShip = Vector3.Distance(monsterTransform.position, shipTransform.position);
            float monsterDetectionDistance = distanceToShip;

            float stalkingThreshold = detectShipValue + (DetectionManager.Instance.CurrentDetectionMultiplier * detectionMultiplier);
            stalkingThreshold = Mathf.Max(stalkingThreshold, 5f);

            //Debug.Log($"{monsterDetectionDistance} <= {stalkingThreshold}");
            if (monsterDetectionDistance <= stalkingThreshold)
            {
                monsterState.SwitchState(monsterState.StalkingState);
            }
        }
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        if (!hasTarget)
        {
            currentTarget = monsterState.GetRandomValidTarget(shipTransform, investigationRadius);
            hasTarget = true;
        }

        float distanceToTarget = Vector3.Distance(monsterTransform.position, currentTarget);
        if (distanceToTarget < 1.5f)
        {
            currentTarget = monsterState.GetRandomValidTarget(shipTransform, investigationRadius);
        }

        Vector3 directionToTarget = (currentTarget - monsterTransform.position).normalized;
        targetDirection = directionToTarget;
        //Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(obstacleAvoidanceDistance);
        //Vector3 combinedDirection = (directionToTarget + obstacleAvoidance).normalized;
        rb.AddForce(directionToTarget * (swimSpeed * DetectionManager.Instance.CurrentDetectionMultiplier), ForceMode.Acceleration);
    }

    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }

    public override void DrawGizmos(MonsterStateMachine monsterState)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(shipTransform.position, investigationRadius);
        Gizmos.DrawSphere(currentTarget, 0.2f);
        Gizmos.DrawLine(monsterTransform.position, monsterTransform.position + monsterTransform.forward * obstacleAvoidanceDistance);
        if (hasTarget)
            Gizmos.DrawLine(monsterTransform.position, shipTransform.position);

#if UNITY_EDITOR
        if (investigationTimer > 0)
        {
            Gizmos.color = Color.yellow;
            float maxBar = 2.0f;
            float ratio = investigationTimer / maxInvestigationTime;
            Vector3 startPos = monsterTransform.position + Vector3.up * 2.0f;
            Vector3 endPos = startPos + Vector3.right * (ratio * maxBar);
            Gizmos.DrawLine(startPos, endPos);

            UnityEditor.Handles.Label(startPos + Vector3.up * 0.3f,
                $"investigating: {investigationTimer:F1}s / {maxInvestigationTime}s");
        }
#endif

        if (isDecoyActive)
        {
            Gizmos.color = Color.yellow;
            float maxBar = 2.0f;
            Vector3 startPos = decoyPosition.position + Vector3.up * 2.0f;
            Vector3 endPos = startPos + Vector3.right * maxBar;
            Gizmos.DrawLine(startPos, endPos);

            UnityEditor.Handles.Label(startPos + Vector3.up * 0.3f,
                "decoy");
        }
    }

    public void SetInvestigationRadius(float radius)
    {
        investigationRadius = baseInvestigationRadius;
        radius *= 0.3f;
        investigationRadius = investigationRadius / radius;
        investigationRadius = Mathf.Round(investigationRadius * 10f) / 10f;
    }

    public void SetRandomValidTarget(Transform target)
    {
        decoyPosition = target;
        isDecoyActive = true;
    }  
}