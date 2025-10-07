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

    float stopAnimation = 0f;

    private static readonly int MaxColliders = 32; 
    private readonly Collider[] ColliderBuffer = new Collider[MaxColliders];

    Animator animator;
    Light lt;

    public float DangerDetectionRadius { get { return dangerDetectionRadius; } set { dangerDetectionRadius = value; } }

    private void Start()
    {
        animator = AnimationController.Instance.GetAnimator(AnimationController.Animators.ShipLampAnimator);
        lt = GetComponentInChildren<Light>();
        lt.range = lightRange;
    }

    private void LateUpdate()
    {
        Vector3 currentPos = transform.position;

        if (!Physics.CheckSphere(currentPos, dangerDetectionRadius))
        {
            lt.range = 0;
            AnimationController.Instance.PlayAnimation(animator, AnimationController.SPEED_MULTIPLIER, stopAnimation);
            return;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(currentPos, dangerDetectionRadius, ColliderBuffer);

        float closestDangerSqr = dangerDetectionRadius * dangerDetectionRadius;
        bool dangerFound = false;

        Vector3 closestDangerDirection = Vector3.zero;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = ColliderBuffer[i];
            if (col == null) continue;

            for (int j = 0; j < TagHolder.dangers.Length; j++)
            {
                if (col.CompareTag(TagHolder.dangers[j]))
                {
                    Vector3 collisionPoint = col.ClosestPoint(currentPos);
                    Vector3 directionToDanger = collisionPoint - currentPos;
                    float distanceToDangerSqr = directionToDanger.sqrMagnitude;

                    if (distanceToDangerSqr < closestDangerSqr)
                    {
                        closestDangerSqr = distanceToDangerSqr;
                        closestDangerDirection = directionToDanger.normalized;
                        dangerFound = true;
                    }
                    break;
                }
            }
        }

        if (dangerFound)
        {
            float closestDanger = Mathf.Sqrt(closestDangerSqr);
            OnDangerDetection?.Invoke(this, (closestDanger, closestDangerDirection));

            float speedFactor = 1f - (closestDanger / dangerDetectionRadius);
            float lerpedAnimSpeed = Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, speedFactor);

            AnimationController.Instance.PlayAnimation(animator, AnimationController.SPEED_MULTIPLIER, lerpedAnimSpeed);
            lt.range = lightRange;
        }
        else
        {
            lt.range = 0f;
            AnimationController.Instance.PlayAnimation(animator, AnimationController.SPEED_MULTIPLIER, stopAnimation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dangerDetectionRadius);

        Collider[] collisions = Physics.OverlapSphere(transform.position, dangerDetectionRadius);

        foreach (Collider collision in collisions)
        {
            for (int i = 0; i < TagHolder.dangers.Length; i++)
            {
                if (collision.CompareTag(TagHolder.dangers[i]))
                {
                    Vector3 collisionPoint = collision.ClosestPoint(transform.position);

                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(transform.position, collisionPoint - transform.position);
                    Gizmos.DrawSphere(collisionPoint, 0.2f);
                }
            }
        }
    }
}