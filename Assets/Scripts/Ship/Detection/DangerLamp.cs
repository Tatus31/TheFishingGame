using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerLamp : MonoBehaviour
{
    [Header("Danger Detection Range")]
    [Space(5)]
    [SerializeField] float dangerDetectionRadius;
    [Header("Animation Value")]
    [Space(5)]
    [SerializeField] float animationSpeed;
    [SerializeField][Range(1,3)] float maxAnimationSpeed;
    [SerializeField][Range(0.1f, 1)] float minAnimationSpeed;
    [SerializeField][Range(0.1f, 1)] float lightRange;

    Animator animator;
    Light lt;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();

        lt = GetComponentInChildren<Light>();
        lt.range = lightRange;
    }

    private void Update()
    {
        Collider[] collisions =  Physics.OverlapSphere(transform.position, dangerDetectionRadius);

        float closestDanger = dangerDetectionRadius;
        bool dangerFound = false;

        if(collisions.Length >= 1)
        {
            Debug.Log("there is a colisision");

            for (int i = 0; i < collisions.Length; i++)
            {
                if (collisions[i].CompareTag(TagHolder.danger))
                {
                    float distanceToDanger = Vector3.Distance(transform.position, collisions[i].transform.position);
                    if (distanceToDanger < closestDanger)
                    {
                        closestDanger = distanceToDanger;
                        dangerFound = true;
                        Debug.Log($"ship is {Mathf.Floor(closestDanger)} away from nearest danger");
                    }
                }
            }

            if (dangerFound)
            {
                float speedFactor = 1 - (closestDanger / dangerDetectionRadius);

                float mappedSpeed = Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, speedFactor);

                AnimationController.Instance.PlayAnimation(animator, AnimationController.SPEED_MULTIPLIER, mappedSpeed);
            }
            else
            {
                lt.range = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, dangerDetectionRadius);
        Gizmos.color = Color.yellow;
    }
}
