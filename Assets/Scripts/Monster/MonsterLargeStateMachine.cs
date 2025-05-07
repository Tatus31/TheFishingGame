using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterLargeStateMachine : MonoBehaviour
{
    public static MonsterLargeStateMachine Instance;

    [Header("[Refrences]")]
    [SerializeField] Transform monsterHead;
    [SerializeField] Transform shipTransform;
    [SerializeField] Transform playerTransform;
    [Header("[General Monster Controls]")]
    [SerializeField] float obstacleAvoidanceDistance = 1f;
    [SerializeField] float obstacleAvoidanceForce = 3f;
    [SerializeField] float swimSpeed = 2f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float maxVelocity = 8f;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask obstacleLayer;
    [Header("[Idle Monster Controls]")]
    [SerializeField] float idleMovementRadius = 10f;
    [SerializeField] float minTimeAtTarget = 0.1f;
    [SerializeField] float allowedDistanceFromTarget = 0.1f;
    [Header("[Stalking Monster Controls]")]
    [SerializeField] float stalkingDistance = 15f;
    [SerializeField] float minStalkingDistance = 8f;
    [SerializeField] float swimStalkingSpeed = 12f;
    [Header("[Attacking Monster Controls]")]
    [SerializeField] float swimAttackSpeed = 20f;
    [SerializeField] float monsterEscapeTime = 2f;
    [SerializeField] float turnSmoothTime = 1.2f;
    [SerializeField] float maxAttackDuration = 6f;
    [SerializeField] int maxNumberOfAttacks = 5;
    [SerializeField] float predictionValue = 1.5f;
    [Header("[Investigating Monster Controls]")]
    [SerializeField] float investigationSwimSpeed = 10f;
    [SerializeField] float visionAngle = 45f;
    [SerializeField] float visionDistance = 15f;

    BaseMonsterState currentState;

    Rigidbody rb;

    public IdleState IdleState { get; private set; }
    public StalkingState StalkingState {  get; private set; }
    public AttackingState AttackingState {  get; private set; }
    public InvestigatingState InvestigatingState { get; private set; }
    public BaseMonsterState PreviousState { get; private set; }
    public BaseMonsterState CurrentState { get; private set; }
    public AttackingPlayerState AttackingPlayerState { get; private set; }
    public Transform ShipTransform { get { return shipTransform; } set { shipTransform = value; } }

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;

        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, waterLayer, rb, monsterHead);
        StalkingState = new StalkingState(shipTransform, monsterHead, rb, minStalkingDistance, swimStalkingSpeed, stalkingDistance, obstacleAvoidanceDistance);
        AttackingState = new AttackingState(shipTransform, monsterHead, playerTransform, swimAttackSpeed, rb, monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue);
        InvestigatingState = new InvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);

        SwitchState(IdleState);
    }

    private void OnValidate()
    {
        if(rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, waterLayer, rb, monsterHead);
        StalkingState = new StalkingState(shipTransform, monsterHead, rb, minStalkingDistance, swimStalkingSpeed, stalkingDistance, obstacleAvoidanceDistance);
        AttackingState = new AttackingState(shipTransform, monsterHead, playerTransform, swimAttackSpeed, rb, monsterEscapeTime, maxAttackDuration, turnSmoothTime, maxNumberOfAttacks, predictionValue);
        InvestigatingState = new InvestigatingState(shipTransform, monsterHead, rb, investigationSwimSpeed, visionAngle, visionDistance);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState(this);

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    public void SwitchState(BaseMonsterState newState)
    {
        ShipDamage shipDamage = ShipDamage.Instance;

        if (shipDamage != null && shipDamage.IsInvincible)
            return;

        currentState?.ExitState();
        PreviousState = currentState;
        currentState = newState;
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

    private void OnDrawGizmos()
    {
        if(monsterHead != null)
            Gizmos.DrawRay(monsterHead.position, Vector3.forward);

        if (currentState != null)
        {
            currentState.DrawGizmos(this);
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

    public bool IsInConeOfVision(Transform origin, Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - origin.position);
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > visionDistance)
            return false;

        directionToTarget.Normalize();
        float dotProduct = Vector3.Dot(origin.forward, directionToTarget);
        float angleToTarget = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        return angleToTarget <= visionAngle;
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
}
