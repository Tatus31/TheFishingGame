using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerLamp : MonoBehaviour
{
    public static event EventHandler<(float distance, Vector3 direction)> OnDangerDetection;

    [Header("Danger Detection Range")]
    [Space(5)]
    [SerializeField] float dangerDetectionRadius;

    [Header("Animation Value")]
    [Space(5)]
    [SerializeField] float animationSpeed;
    [SerializeField][Range(1, 3)] float maxAnimationSpeed;
    [SerializeField][Range(0.1f, 1)] float minAnimationSpeed;
    [SerializeField][Range(0.1f, 1)] float lightRange;

    Animator animator;
    Light lt;

    public float DangerDetectionRadius { get { return dangerDetectionRadius; } set { dangerDetectionRadius = value; } }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        lt = GetComponentInChildren<Light>();
        lt.range = lightRange;
    }

    private void Update()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, dangerDetectionRadius);

        float closestDanger = dangerDetectionRadius;
        bool dangerFound = false;

        Vector3 closestDangerDirection = Vector3.zero;
        Vector3 closestCollisionPoint = Vector3.zero;

        if (collisions.Length >= 1)
        {
            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].CompareTag(TagHolder.danger))
                {
                    Vector3 collisionPoint = collisions[i].ClosestPoint(transform.position);
                    float distanceToDanger = Vector3.Distance(transform.position, collisionPoint);

                    Debug.Log($"Closest point is {Mathf.Floor(distanceToDanger)} away from danger");

                    if (distanceToDanger < closestDanger)
                    {
                        closestDanger = distanceToDanger;
                        closestDangerDirection = (collisionPoint - transform.position).normalized;
                        closestCollisionPoint = collisionPoint;
                        dangerFound = true;

                        Debug.Log($"Nearest danger point is {Mathf.Floor(closestDanger)} away");
                    }
                }
            }

            if (dangerFound)
            {
                OnDangerDetection?.Invoke(this, (closestDanger, closestDangerDirection));

                float speedFactor = 1 - (closestDanger / dangerDetectionRadius);
                float lerpedAnimSpeed = Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, speedFactor);
                AnimationController.Instance.PlayAnimation(animator, AnimationController.SPEED_MULTIPLIER, lerpedAnimSpeed);

                lt.range = lightRange;
            }
            else
            {
                lt.range = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dangerDetectionRadius);

        Collider[] collisions = Physics.OverlapSphere(transform.position, dangerDetectionRadius);

        foreach (Collider collision in collisions)
        {
            if (collision.CompareTag(TagHolder.danger))
            {
                Vector3 collisionPoint = collision.ClosestPoint(transform.position);

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, collisionPoint - transform.position);
                Gizmos.DrawSphere(collisionPoint, 0.2f);
            }
        }
    }
}