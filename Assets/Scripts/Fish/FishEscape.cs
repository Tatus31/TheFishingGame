using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEscape : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] LayerMask playerMask, waterLayer;
    [SerializeField] float fleeSpeed = 5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float sphereRadius = 2f;
    [SerializeField] float bounceAngleModifier = 0.5f;

    float fleeTimer = 0f;
    float maxFleeTimer = 0.5f;
    bool fleeing;

    Vector3 lastVelocity;
    Vector3 targetDirection;

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

        lastVelocity = rb.velocity;
    }

    private void FixedUpdate()
    {
        Collider[] collider = Physics.OverlapSphere(transform.localPosition, sphereRadius, playerMask);
        if (collider.Length >= 1 && fleeTimer < maxFleeTimer)
        {
            fleeing = true;
            Vector3 playerPosition = collider[0].transform.position;
            Vector3 fleeDirection = (transform.position - playerPosition).normalized;
            rb.AddForce(fleeDirection * fleeSpeed, ForceMode.Acceleration);
            targetDirection = fleeDirection;
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

    private void OnCollisionEnter(Collision collision)
    {
        float speed = lastVelocity.magnitude;
        Vector3 normal = collision.contacts[0].normal;

        Vector3 reflectedDirection = Vector3.Reflect(lastVelocity.normalized, normal);
        Vector3 adjustedDirection = Vector3.Lerp(reflectedDirection, normal, bounceAngleModifier).normalized;
        adjustedDirection = Quaternion.Euler(0, Random.Range(-15f, 15f), 0) * adjustedDirection;

        rb.velocity = adjustedDirection * Mathf.Max(speed, 0f);
        targetDirection = adjustedDirection;
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
        Gizmos.DrawWireSphere(transform.localPosition, 2f);
    }
}