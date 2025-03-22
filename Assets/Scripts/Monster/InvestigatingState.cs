using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatingState : BaseMonsterState
{
    Transform shipTransform;
    Transform monsterTransform;

    float investigationRadius = 50f;
    float swimSpeed = 2f;
    float obstacleAvoidanceDistance = 1f;

    Vector3 currentTarget;
    Vector3 targetDirection;

    bool hasTarget;

    Rigidbody rb;

    public InvestigatingState(Transform shipTransform, Transform monsterTransform, float investigationRadius, float swimSpeed, float obstacleAvoidanceDistance, Rigidbody rb)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.investigationRadius = investigationRadius;
        this.swimSpeed = swimSpeed;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;
        this.rb = rb;
    }


    public override void EnterState(MonsterStateMachine monsterState)
    {
        Debug.Log($"Investigating in a radius of {investigationRadius}");

        hasTarget = false;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        monsterState.LookAtTarget(targetDirection);
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        if (!hasTarget)
        {
            currentTarget = monsterState.GetRandomValidTarget(shipTransform, investigationRadius);
            hasTarget = true;
        }

        Vector3 directionToTarget = (currentTarget - monsterTransform.position).normalized;
        //Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(obstacleAvoidanceDistance);
        Vector3 combinedDirection = (directionToTarget/* + obstacleAvoidance*/).normalized;

        rb.AddForce(combinedDirection * swimSpeed, ForceMode.Acceleration);
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
    }

    public void SetInvestigationRadius(float radius)
    {
        investigationRadius = investigationRadius / radius;
        investigationRadius = Mathf.Round(investigationRadius * 10f) / 10f;
    }
}
