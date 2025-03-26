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

    float highDetectionTimer = 0f; 
    float lowDetectionTimer = 0f;  

    float detectionThresholdTime = 15f;
    float highDetectionThreshold = 1.9f;
    float lowDetectionThreshold = 0.5f;

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

        AudioManager.MuteSound(AudioManager.HeartBeatSound);
        shipMovement.OnShipSpeedChange += OnShipSpeedChanged;
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

        UpdateDetectionThreshold();

        float currentDetectionMultiplier = DetectionManager.Instance.CurrentDetectionMultiplier;

        if (currentDetectionMultiplier > highDetectionThreshold)
        {
            highDetectionTimer += Time.deltaTime;
            lowDetectionTimer = 0f;

            if (highDetectionTimer >= detectionThresholdTime && !isTransitioning)
            {
                isTransitioning = true;
                highDetectionTimer = 0f;

                monsterState.SwitchState(monsterState.AttackingState);
                return;
            }
        }
        else
        {
            highDetectionTimer = Mathf.Max(0, highDetectionTimer - (Time.deltaTime * 0.5f));
        }

        if (currentDetectionMultiplier <= lowDetectionThreshold)
        {
            lowDetectionTimer += Time.deltaTime;

            if (lowDetectionTimer >= detectionThresholdTime && !isTransitioning)
            {
                isTransitioning = true;
                lowDetectionTimer = 0f;

                monsterState.SwitchState(monsterState.IdleState);
                DetectionManager.Instance.IsInvestigating = false;
                return;
            }
        }
        else
        {
            lowDetectionTimer = Mathf.Max(0, lowDetectionTimer - (Time.deltaTime * 0.5f));
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

            if (highDetectionTimer > 0)
            {
                Gizmos.color = Color.red;
                float progress = highDetectionTimer / detectionThresholdTime;
                float barLength = 2.0f;
                Vector3 startPos = monsterTransform.position + Vector3.up * 1.5f;
                Vector3 endPos = startPos + Vector3.right * (progress * barLength);

#if UNITY_EDITOR
                UnityEditor.Handles.Label(endPos, $"{Mathf.Round(progress * 100)}%");
                float closeRange = 10f;
                float mediumRange = 30f;
                float distanceToShip = Vector3.Distance(monsterTransform.position, shipTransform.position);
                float t = (distanceToShip - closeRange) / (mediumRange - closeRange);
                UnityEditor.Handles.Label(endPos + Vector3.up * 1.5f, $"{Mathf.Round(t * 100) / 100}");
#endif
            }

            if (lowDetectionTimer > 0)
            {
                Gizmos.color = Color.blue;
                float progress = lowDetectionTimer / detectionThresholdTime;
                float barLength = 2.0f;
                Vector3 startPos = monsterTransform.position + Vector3.up * 1.2f;
                Vector3 endPos = startPos + Vector3.right * (progress * barLength);

#if UNITY_EDITOR
                UnityEditor.Handles.Label(endPos, $"{Mathf.Round(progress * 100)}%");
#endif
            }
        }
    }

    void UpdateDetectionThreshold()
    {
        float distanceToShip = Vector3.Distance(monsterTransform.position, shipTransform.position);

        float maxThreshold = 1.9f;

        float closeRange = 10f;    
        float mediumRange = 30f;   

        if (distanceToShip <= mediumRange)
        {
            float t = (distanceToShip - closeRange) / (mediumRange - closeRange);
            highDetectionThreshold = t;
        }
        else
        {
            highDetectionThreshold = maxThreshold;
        }
    }

    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {

    }
}