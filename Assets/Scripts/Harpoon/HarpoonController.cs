using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonController : MonoBehaviour
{
    [SerializeField] float maxHarpoonDistance = 20f;
    [SerializeField] float harpoonPullForce = 20f;
    [SerializeField] float harpoonMaxSpeed = 15f;
    [SerializeField] float minReturnDelay = 0.1f, maxReturnDelay = 1f;
    [SerializeField] LayerMask harpoonLayerMask;
    [SerializeField] float detachDistance = 25f; 

    Rigidbody rb;

    Vector3 hitPoint;
    Vector3 hookPoint;

    bool isHooked;

    LineRenderer lineRenderer;

    Coroutine returnCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (!InteractionManager.Instance.HasHarpoon)
            return;

        if (InputManager.Instance.IsLeftMouseButtonHeld())
        {
            if (!isHooked)
            {

                hitPoint = MouseWorldPosition.GetMouseWorldPosition(maxHarpoonDistance, harpoonLayerMask);

                if (hitPoint != Vector3.zero)
                    TryHooking(hitPoint);
            }
            else
                PullTowardsHarpoonHook();
        }
        else if (InputManager.Instance.IsRightMouseButtonPressed() && isHooked)
            StartCoroutine(ReturnHarpoon());

        UpdateHarpoonLine();
    }

    void TryHooking(Vector3 point)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (point - transform.position).normalized, out hit, maxHarpoonDistance, harpoonLayerMask))
        {
            isHooked = true;
            hookPoint = hit.point;
            lineRenderer.enabled = true;
        }
    }

    void PullTowardsHarpoonHook()
    {
        if (!isHooked)
            return;

        Vector3 direction = (hookPoint - rb.position).normalized;
        float distance = Vector3.Distance(hookPoint, rb.position);

        if (distance > 1f) 
        {
            if (rb.velocity.magnitude <= harpoonMaxSpeed)
            {
                rb.AddForce(direction * harpoonPullForce, ForceMode.Acceleration);
            }
        }
    }

    void UpdateHarpoonLine()
    {
        if (!isHooked)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.material.color = Color.black;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hookPoint);

        if (Vector3.Distance(transform.position, hookPoint) > detachDistance)
        {
            DetachHarpoon();
        }
    }

    void DetachHarpoon()
    {
        isHooked = false;
        lineRenderer.enabled = false;
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    IEnumerator ReturnHarpoon()
    {
        float distance = Vector3.Distance(transform.position, hookPoint);
        float dynamicReturnDelay = Mathf.Lerp(minReturnDelay, maxReturnDelay, distance / maxHarpoonDistance);

        yield return new WaitForSeconds(dynamicReturnDelay);
        DetachHarpoon();
    }
}