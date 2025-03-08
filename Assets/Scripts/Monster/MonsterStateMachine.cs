using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MonsterStateMachine : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Transform monsterHead;

    [Header("Idle Monster Controls")]
    [SerializeField] float idleMovementRadius = 10f;
    [SerializeField] float obstacleAvoidanceDistance = 1f;
    [SerializeField] float obstacleAvoidanceForce = 3f;
    [SerializeField] float swimSpeed = 2f;
    [SerializeField] float minTimeAtTarget = 0.1f;
    [SerializeField] float allowedDistanceFromTarget = 0.1f;
    [SerializeField] float rotationSpeed = 5f;

    [SerializeField] LayerMask waterLayer;
    [SerializeField] LayerMask obstacleLayer;

    BaseMonsterState currentState;

    Rigidbody rb;

    public IdleState IdleState { get; private set; }
    public StalkingState stalkingState = new StalkingState();
    public AttackingState attackingState = new AttackingState();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, obstacleAvoidanceForce, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rotationSpeed, waterLayer, obstacleLayer, rb, monsterHead);

        SwitchState(IdleState);
    }

    private void OnValidate()
    {
        IdleState = new IdleState(idleMovementRadius, obstacleAvoidanceDistance, obstacleAvoidanceForce, swimSpeed, minTimeAtTarget, allowedDistanceFromTarget, rotationSpeed, waterLayer, obstacleLayer, rb, monsterHead);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }

    public void SwitchState(BaseMonsterState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }

    private void OnDrawGizmos()
    {
        if (currentState != null)
        {
            currentState.DrawGizmos(this);
        }
    }
}
