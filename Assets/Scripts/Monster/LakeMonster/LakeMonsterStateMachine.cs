using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LakeMonsterStateMachine : MonoBehaviour
{
    //public static MediumMonsterStateMachine Instance;

    [Header("[References]")]
    [SerializeField] public Transform monsterHead;
    [SerializeField] Transform shipTransform;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform safeSpaceCenter;

    [Header("[General Monster Controls]")]
    [SerializeField] public float obstacleAvoidanceDistance = 1f;
    [SerializeField] float obstacleAvoidanceForce = 3f;
    [SerializeField] float swimSpeed = 3f;
    [SerializeField] float rotationSpeed = 6f;
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] public LayerMask obstacleLayer;

    [Header("[Idle Monster Controls]")]
    [SerializeField] float idleMovementRadius = 12f;
    [SerializeField] float minTimeAtTarget = 1.5f;
    [SerializeField] float allowedDistanceFromTarget = 0.5f;

    [Header("[Investigating Monster Controls]")]
    [SerializeField] float investigationSwimSpeed = 10f;
    [SerializeField] float visionAngle = 45f;
    [SerializeField] float visionDistance = 15f;

    [Header("[Attacking Monster Controls]")]
    [SerializeField] float monsterEscapeTime = 2f;
    [SerializeField] float swimAttackSpeed = 20f;
    [SerializeField] float maxAttackDuration = 6f;
    [SerializeField] int maxNumberOfAttacks = 5;
    [SerializeField] float turnSmoothTime = 1.2f;
    [SerializeField] float predictionValue = 1.5f;
    [SerializeField] float windUpDuration = 1.5f;
    [SerializeField] float windUpRotationSpeed = 2f;

    [Space(10)]
    [SerializeField] public bool IsSmallMonster = true;

    BaseLakeMonsterState currentState;
    Rigidbody rb;
    ShipMovement shipMovement;

    public LakeMonsterIdleState IdleState { get; private set; }
    public LakeMonsterInvestigatingState InvestigatingState { get; private set; }
    public LakeMonsterAttackingState AttackingState { get; private set; }
    public BaseLakeMonsterState PreviousState { get; private set; }
    public BaseLakeMonsterState CurrentState { get; private set; }
    public Transform ShipTransform { get { return shipTransform; } set { shipTransform = value; } }

    Vector3 _smoothedAvoidance = Vector3.zero; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        IdleState = new LakeMonsterIdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rb);
        InvestigatingState = new LakeMonsterInvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);
        AttackingState = new LakeMonsterAttackingState(shipTransform, monsterHead, playerTransform, this.shipMovement, swimAttackSpeed, rb,
            monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue, windUpDuration, windUpRotationSpeed);

        if(shipTransform.TryGetComponent<ShipMovement>(out ShipMovement shipMovement))
        {
            this.shipMovement = shipMovement;
        }

        SwitchState(IdleState);
    }

    private void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        IdleState = new LakeMonsterIdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rb);
        InvestigatingState = new LakeMonsterInvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);
        AttackingState = new LakeMonsterAttackingState(shipTransform, monsterHead, playerTransform, this.shipMovement, swimAttackSpeed,
            rb, monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue, windUpDuration, windUpRotationSpeed);
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    private void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdateState(this);
        }

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    public void SwitchState(BaseLakeMonsterState newState)
    {
        if (ShipDamage.Instance != null && ShipDamage.Instance.IsInvincible)
            return;

        currentState?.ExitState();
        PreviousState = currentState;
        currentState = newState;
        CurrentState = newState;
        currentState.EnterState(this);
    }

    public void OnTriggerEnter(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = true;
        }
    }

    public Vector3 GetObstacleAvoidanceDirection(float obstacleAvoidanceDistance)
    {
        Vector3 avoidanceDirection = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(monsterHead.position, monsterHead.forward, out hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            avoidanceDirection = Vector3.Reflect(monsterHead.forward, hit.normal).normalized;
            avoidanceDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * avoidanceDirection;
        }

        return avoidanceDirection * obstacleAvoidanceForce;
    }

    public Vector3 GetSmartAvoidanceDirection()
    {
        float sphereRadius = 0.5f;
        float weightMultiplier = 2f;
        int totalSensors = 5;

        Vector3[] directions = {monsterHead.forward,Quaternion.Euler(0, -30f, 0) * monsterHead.forward,Quaternion.Euler(0, 30f, 0) * monsterHead.forward,Quaternion.Euler(-25f, 0, 0) * monsterHead.forward,Quaternion.Euler(25f, 0, 0) * monsterHead.forward};

        Vector3 avoidance = Vector3.zero;
        int hitCount = 0;
        float closestHit = float.MaxValue;
        Vector3 closestNormal = Vector3.zero;

        foreach (Vector3 dir in directions)
        {
            if (Physics.SphereCast(monsterHead.position, sphereRadius, dir, out RaycastHit hit, obstacleAvoidanceDistance, obstacleLayer))
            {
                float proximity = 1f - (hit.distance / obstacleAvoidanceDistance);
                Vector3 awayFromObstacle = hit.normal * proximity * weightMultiplier;
                avoidance += awayFromObstacle;
                hitCount++;

                if (hit.distance < closestHit)
                {
                    closestHit = hit.distance;
                    closestNormal = hit.normal;
                }

#if UNITY_EDITOR
                Debug.DrawLine(monsterHead.position, hit.point, Color.red);
#endif
            }
            else
            {
#if UNITY_EDITOR
                Debug.DrawRay(monsterHead.position, dir * obstacleAvoidanceDistance, Color.green);
#endif
            }
        }

        bool isFullyBlocked = (hitCount == totalSensors); 
        bool isMostlyBlocked = (hitCount >= totalSensors - 1 && closestHit < obstacleAvoidanceDistance * 0.3f);

        if (isFullyBlocked || isMostlyBlocked)
        {
            Vector3 retreat = -monsterHead.forward + closestNormal;
            _smoothedAvoidance = Vector3.Slerp(_smoothedAvoidance, retreat.normalized * obstacleAvoidanceForce, Time.deltaTime * 4f);
            return _smoothedAvoidance;
        }

        if (hitCount > 0)
        {
            avoidance /= hitCount;

            Vector3 slideDir = Vector3.ProjectOnPlane(monsterHead.forward, closestNormal).normalized;
            Vector3 combined = Vector3.Slerp(avoidance.normalized, slideDir, 0.4f);

            _smoothedAvoidance = Vector3.Slerp(_smoothedAvoidance, combined * obstacleAvoidanceForce, Time.deltaTime * 5f);
        }
        else
        {
            _smoothedAvoidance = Vector3.Slerp(_smoothedAvoidance, Vector3.zero, Time.deltaTime * 2f);
        }

        return _smoothedAvoidance;
    }

    public bool IsInConeOfVision(Transform origin, Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - origin.position);

        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > visionDistance)
            return false;

        directionToTarget.Normalize();

        float dotProduct = Vector3.Dot(origin.forward, directionToTarget);
        float angleToTarget = Mathf.Acos(Mathf.Clamp(dotProduct, -1f, 1f)) * Mathf.Rad2Deg;

        return angleToTarget <= visionAngle;
    }

    public void LookAt(Vector3 lookAtDirection)
    {
        if (lookAtDirection != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(lookAtDirection);
            monsterHead.rotation = Quaternion.Slerp(monsterHead.rotation, targetLookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public Vector3 GetRandomValidTarget(Transform transform, float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * radius;

            if (Physics.CheckSphere(randomPoint, 0.1f, waterLayer) && !Physics.CheckSphere(randomPoint, 10f, obstacleLayer))
            {
                return randomPoint;
            }
        }

        return transform.position;
    }

    public Vector3 GetRandomValidTargetInsideSafeSpace(float radius)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = safeSpaceCenter.position + Random.insideUnitSphere * radius;

            if (Physics.CheckSphere(randomPoint, 0.1f, waterLayer) && !Physics.CheckSphere(randomPoint, 10f, obstacleLayer))
            {
                return randomPoint;
            }
        }

        return transform.position;
    }

    public void LookAtTarget(Vector3 targetDirection)
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            LookAt(targetDirection);
        }
    }

    public float GetDistanceToShip()
    {
        return Vector3.Distance(monsterHead.position, shipTransform.position);
    }

    private void OnDrawGizmos()
    {
        if (monsterHead != null)
            Gizmos.DrawRay(monsterHead.position, monsterHead.forward * 2f);

        if (currentState != null)
        {
            currentState.DrawGizmos(this);
        }
    }
}