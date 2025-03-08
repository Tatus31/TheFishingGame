using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterStateMachine : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Transform monsterHead;
    [SerializeField] Transform shipTransform;
    [Header("General Monster Controls")]
    [SerializeField] float obstacleAvoidanceDistance = 1f;
    [SerializeField] float obstacleAvoidanceForce = 3f;
    [SerializeField] float swimSpeed = 2f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float maxVelocity = 8f;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask obstacleLayer;
    [Header("Idle Monster Controls")]
    [SerializeField] float idleMovementRadius = 10f;
    [SerializeField] float minTimeAtTarget = 0.1f;
    [SerializeField] float allowedDistanceFromTarget = 0.1f;
    [Header("Stalking Monster Controls")]
    [SerializeField] float stalkingDistance = 15f;
    [SerializeField] float minStalkingDistance = 8f;
    [SerializeField] float swimStalkingSpeed = 12f;

    BaseMonsterState currentState;

    Rigidbody rb;

    public IdleState IdleState { get; private set; }
    public StalkingState StalkingState {  get; private set; }
    public AttackingState attackingState = new AttackingState();

    public Transform ShipTransform { get { return shipTransform; } set { shipTransform = value; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, waterLayer, rb, monsterHead);
        StalkingState = new StalkingState(shipTransform, monsterHead, rb, minStalkingDistance, swimStalkingSpeed, stalkingDistance);

        SwitchState(IdleState);
    }

    private void OnValidate()
    {
        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, waterLayer, rb, monsterHead);
        StalkingState = new StalkingState(shipTransform, monsterHead, rb, minStalkingDistance, swimStalkingSpeed, stalkingDistance);
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
        currentState?.ExitState();
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
        if (currentState != null)
        {
            currentState.DrawGizmos(this);
        }
    }

    public Vector3 GetObstacleAvoidanceDirection()
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
}
