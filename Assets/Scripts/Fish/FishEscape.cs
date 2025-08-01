using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishEscape : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] LayerMask playerMask, waterLayer, obstacleLayer;
    [SerializeField] float fleeSpeed = 5f;
    [SerializeField] float swimSpeed = 2f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float sphereRadius = 2f;
    [SerializeField] float bounceAngleModifier = 0.5f;
    [SerializeField] float minTimeAtTarget = 1f;
    [SerializeField] float allowedDistanceFromTarget = 0.1f;
    [SerializeField] float obstacleAvoidanceDistance = 1f; 
    [SerializeField] float obstacleAvoidanceForce = 3f;

    float fleeTimer = 0f;
    float maxFleeTimer = 2f;
    float timeAtTarget = 0f;

    bool fleeing;
    bool hasTarget;

    Vector3 lastVelocity;
    Vector3 targetDirection;
    Vector3 currentTarget;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (fleeTimer >= maxFleeTimer)
        {
            fleeTimer = 0f;
            fleeing = false;
        }
        if (fleeing)
        {
            startTimer();
        }

        if (rb.velocity.magnitude > 0.1f)
        {
            targetDirection = rb.velocity.normalized;
            LookAt(targetDirection);
        }

        if (hasTarget)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
            if (distanceToTarget < allowedDistanceFromTarget)
            {
                timeAtTarget += Time.deltaTime;
                if (timeAtTarget >= minTimeAtTarget)
                {
                    hasTarget = false;
                    timeAtTarget = 0f;
                }
            }
        }

        lastVelocity = rb.velocity;
    }

    private void FixedUpdate()
    {
        Collider[] colliderPlayer = Physics.OverlapSphere(transform.position, sphereRadius, playerMask);

        if (colliderPlayer.Length >= 1 && fleeTimer < maxFleeTimer)
        {
            fleeing = true;

            Vector3 playerPosition = colliderPlayer[0].transform.position;
            Vector3 fleeDirection = (transform.position - playerPosition).normalized;
            Vector3 obstacleAvoidance = GetObstacleAvoidanceDirection();
            Vector3 combinedDirection = (fleeDirection + obstacleAvoidance).normalized;

            rb.AddForce(combinedDirection * fleeSpeed, ForceMode.Acceleration);
            targetDirection = combinedDirection;
        }
        else if (!fleeing)
        {
            if (!hasTarget)
            {
                Vector3 newTarget = GetRandomValidTarget();
                if (newTarget != Vector3.zero)
                {
                    currentTarget = newTarget;
                    hasTarget = true;
                }
            }
            else
            {
                Vector3 directionToTarget = (currentTarget - transform.position).normalized;
                Vector3 obstacleAvoidance = GetObstacleAvoidanceDirection();
                Vector3 combinedDirection = (directionToTarget + obstacleAvoidance).normalized;

                rb.AddForce(combinedDirection * swimSpeed, ForceMode.Acceleration);
            }
        }
    }

    void startTimer()
    {
        fleeTimer += Time.deltaTime;
    }

    void LookAt(Vector3 lookAtDirection)
    {
        if (lookAtDirection != Vector3.zero)
        {
            Quaternion targetLookRotation = Quaternion.LookRotation(lookAtDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetLookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    Vector3 GetRandomValidTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * sphereRadius;

            RaycastHit hit;
            Vector3 directionToPoint = (randomPoint - transform.position).normalized;
            float distanceToPoint = Vector3.Distance(transform.position, randomPoint);

            if (!Physics.Raycast(transform.position, directionToPoint, out hit, distanceToPoint, waterLayer))
            {
                return randomPoint;
            }
        }

        return Vector3.zero;
    }

    Vector3 GetObstacleAvoidanceDirection()
    {
        Vector3 avoidanceDirection = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance, obstacleLayer))
        {
            avoidanceDirection = Vector3.Reflect(transform.forward, hit.normal).normalized;
            avoidanceDirection = Quaternion.Euler(0, Random.Range(-45f, 45f), 0) * avoidanceDirection;
        }

        return avoidanceDirection * obstacleAvoidanceForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float speed = lastVelocity.magnitude;
        Vector3 normal = collision.contacts[0].normal;

        Vector3 reflectedDirection = Vector3.Reflect(lastVelocity.normalized, normal);
        Vector3 adjustedDirection = Vector3.Lerp(reflectedDirection, normal, bounceAngleModifier).normalized;
        adjustedDirection = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * adjustedDirection;

        rb.velocity = adjustedDirection * Mathf.Max(speed, 0f);
        targetDirection = adjustedDirection;

        hasTarget = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((waterLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            rb.useGravity = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
        Gizmos.DrawSphere(currentTarget, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * obstacleAvoidanceDistance);
    }
}