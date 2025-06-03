using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MediumMonsterStateMachine : MonoBehaviour
{
    //public static MediumMonsterStateMachine Instance;

    [Header("[References]")]
    [SerializeField] public Transform monsterHead;
    [SerializeField] Transform shipTransform;
    [SerializeField] Transform playerTransform;

    [Header("[General Monster Controls]")]
    [SerializeField] public float obstacleAvoidanceDistance = 1f;
    [SerializeField] float obstacleAvoidanceForce = 3f;
    [SerializeField] float swimSpeed = 3f;
    [SerializeField] float rotationSpeed = 6f;
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask obstacleLayer;

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

    BaseMediumMonsterState currentState;
    Rigidbody rb;

    public MediumMonsterIdleState IdleState { get; private set; }
    public MediumMonsterInvestigatingState InvestigatingState { get; private set; }
    public MediumMonsterAttackingState AttackingState { get; private set; }
    public BaseMediumMonsterState PreviousState { get; private set; }
    public BaseMediumMonsterState CurrentState { get; private set; }

    public Transform ShipTransform { get { return shipTransform; } set { shipTransform = value; } }

    public bool isSmallMonster = true; 

    private void Awake()
    {
//        if (Instance != null)
//        {
//#if UNITY_EDITOR
//            Debug.LogWarning($"There exists a {Instance.name} in the scene already");
//#endif
//        }

//        Instance = this;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        IdleState = new MediumMonsterIdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rb);
        InvestigatingState = new MediumMonsterInvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);
        AttackingState = new MediumMonsterAttackingState(shipTransform, monsterHead, playerTransform, swimAttackSpeed, rb, monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue);

        SwitchState(IdleState);
    }

    private void OnValidate()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        IdleState = new MediumMonsterIdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rb);
        InvestigatingState = new MediumMonsterInvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);
        AttackingState = new MediumMonsterAttackingState(shipTransform, monsterHead, playerTransform, swimAttackSpeed, rb, monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue);
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

    public void SwitchState(BaseMediumMonsterState newState)
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

            if (Physics.CheckSphere(randomPoint, 0.1f, waterLayer))
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