using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkingState : BaseMonsterState
{
    float stalkingDistance = 15f;
    float minStalkingDistance = 8f;
    float swimStalkingSpeed = 12f;

    float shipVelPrediction = 1.0f;
    float velDamping = 0.95f;

    float approachSpeedFactor = 0.1f;
    float slowdownDistance = 4f;
    float stayPutTreshold = 5f;
    float stacionaryShipVel = 0.3f;

    float obstacleAvoidanceDistance = 1f;

    float attackTimer = 0f;
    float attackTimerThreshold = 10f; 

    bool isTransitioning = false;

    Transform shipTransform;
    Transform monsterTransform;

    ShipMovement shipMovement;

    Rigidbody rb;

    Vector3 targetDirection;
    Vector3 shipVel;

    public StalkingState(Transform shipTransform, Transform monsterTransform, Rigidbody rb, float minStalkingDistance, float swimStalkingSpeed, float stalkingDistance, float obstacleAvoidanceDistance)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.rb = rb;
        this.stalkingDistance = stalkingDistance;
        this.minStalkingDistance = minStalkingDistance;
        this.swimStalkingSpeed = swimStalkingSpeed;
        this.obstacleAvoidanceDistance = obstacleAvoidanceDistance;

        if (shipTransform != null)
        {
            shipMovement = shipTransform.GetComponent<ShipMovement>();
            if (shipMovement == null)
                return;
        }
    }

    public override void EnterState(MonsterStateMachine monsterState)
    {
        shipVel = Vector3.zero;
        isTransitioning = false;

        attackTimer = 0f;

        if (shipMovement != null)
        {
            shipMovement.OnShipSpeedChange += OnShipSpeedChanged;
        }

        AudioManager.PlaySound(AudioManager.HeartBeatSound);
        AudioManager.MuteSound(AudioManager.HeartBeatSlowSound);
    }

    public override void ExitState()
    {
        if (shipMovement != null)
        {
            shipMovement.OnShipSpeedChange -= OnShipSpeedChanged;
        }

        Debug.Log("Exiting Stalking State");
    }

    private void OnShipSpeedChanged(object sender, Vector3 velocity)
    {
        shipVel = velocity;
    }

    public override void UpdateState(MonsterStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            monsterState.LookAt(targetDirection);
        }

        if (shipMovement != null)
        {
            shipVel = shipMovement.ShipFlatVel;
        }

        if (isTransitioning)
            return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackTimerThreshold)
        {
            monsterState.SwitchState(monsterState.AttackingState);
            return;
        }
    }

    public override void FixedUpdateState(MonsterStateMachine monsterState)
    {
        if (shipTransform == null || monsterTransform == null)
            return;

        bool isShipStationary = shipVel.magnitude < stacionaryShipVel;

        float predictionFactor = isShipStationary ? 0f : shipVelPrediction;
        Vector3 predictedShipPosition = shipTransform.position + (shipVel * predictionFactor);

        Vector3 directionToShip = predictedShipPosition - monsterTransform.position;
        float distanceToShip = directionToShip.magnitude;

        Vector3 normalizedDirectionToShip = distanceToShip > 0.001f ?
            directionToShip / distanceToShip : Vector3.zero;

        Vector3 targetPosition = predictedShipPosition - (normalizedDirectionToShip * stalkingDistance);
        Vector3 directionToTarget = targetPosition - monsterTransform.position;
        float distanceToTarget = directionToTarget.magnitude;

        Vector3 moveForce = Vector3.zero;

        float distanceMultiplier = Mathf.Clamp(distanceToTarget / stalkingDistance, 0.5f, 2.0f);

        if (distanceToShip < minStalkingDistance)
        {
            moveForce = -normalizedDirectionToShip * swimStalkingSpeed * 2.0f;
        }
        else if (distanceToTarget > stayPutTreshold)
        {
            float approachSpeed = swimStalkingSpeed;

            approachSpeed *= distanceMultiplier;

            if (distanceToTarget < slowdownDistance)
            {
                approachSpeed = Mathf.Lerp(swimStalkingSpeed, approachSpeed, distanceToTarget / slowdownDistance);
            }

            if (isShipStationary)
            {
                approachSpeed *= approachSpeedFactor;
            }

            moveForce = directionToTarget.normalized * approachSpeed;
        }
        else if (!isShipStationary)
        {
            moveForce = shipVel.normalized * swimStalkingSpeed * velDamping;
        }
        else
        {
            moveForce = Vector3.zero;

            rb.velocity *= velDamping;
        }

        Vector3 obstacleAvoidance = monsterState.GetObstacleAvoidanceDirection(obstacleAvoidanceDistance);
        Vector3 combinedDirection = (moveForce + obstacleAvoidance).normalized;

        if (moveForce.magnitude > 0.01f)
        {
            rb.AddForce(combinedDirection * moveForce.magnitude, ForceMode.Acceleration);
        }
    }

    public override void DrawGizmos(MonsterStateMachine monsterState)
    {
        if (shipTransform != null && monsterTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(shipTransform.position, stalkingDistance);

            Vector3 dirToShip = (shipTransform.position - monsterTransform.position).normalized;
            Vector3 targetPos = shipTransform.position - (dirToShip * stalkingDistance);
            Gizmos.DrawWireSphere(targetPos, slowdownDistance);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(monsterTransform.position, shipTransform.position);

            bool isShipStationary = shipVel.magnitude < stacionaryShipVel;
            float predictionFactor = isShipStationary ? 0f : shipVelPrediction;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(shipTransform.position + (shipVel * predictionFactor), 0.5f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(targetPos, 0.5f);

            Vector3 timerLabelPos = monsterTransform.position + Vector3.up * 2f;
            string timerLabel = $"Attack Timer: {attackTimer:F1}/{attackTimerThreshold:F1}";
            UnityEditor.Handles.Label(timerLabelPos, timerLabel);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}