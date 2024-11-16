using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    [SerializeField] float lineLength = 10.0f;
    [SerializeField] Color lineColor = Color.red;
    [SerializeField] Transform orientation;
    [SerializeField] float trajectoryHeight = 2.0f;

    private bool hasThrown = false;
    private Vector3 throwVelocity;
    private Vector3 startPosition;

    InputManager inputManager;

    void Start()
    {
        inputManager = InputManager.Instance;
    }

    void Update()
    {
        if (inputManager.IsLeftMouseButtonPressed())
        {
            startPosition = transform.position;
            throwVelocity = CalculateThrowVelocity(startPosition, MouseWorldPosition.GetMouseWorldPosition(lineLength), trajectoryHeight);

            hasThrown = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Vector3 startPosition = transform.position + Vector3.up * 0.5f;
        Vector3 forwardDirection = orientation.forward;

        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 endPosition = startPosition + forwardDirection * lineLength;

        Gizmos.DrawLine(startPosition, endPosition);

        if (hasThrown)
        {
            Gizmos.color = Color.blue;
            Vector3 position = startPosition;
            Vector3 velocity = throwVelocity;
            float timeStep = 0.1f;

            for (float t = 0; t < 2f; t += timeStep)
            {
                Vector3 nextPosition = position + velocity * timeStep;
                velocity += Physics.gravity * timeStep;

                Gizmos.DrawLine(position, nextPosition);
                position = nextPosition;
            }
        }
    }

    public Vector3 CalculateThrowVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
