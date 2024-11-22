using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Throw : MonoBehaviour
{
    public event EventHandler<float> OnThrowStrenghtChange;
    public event EventHandler<bool> OnThrowing;

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Transform hitVisual;

    [Header("Throw Settings")]
    [SerializeField] float maxLineLength = 10f;
    [SerializeField] float minLineLength = 1f;
    [SerializeField] float lineGrowthRate = 5f;
    [Space(10)]
    [SerializeField] float minTrajectoryHeight = 1f;

    [Header("Gizmo Visuals")]
    [SerializeField] Color lineColor = Color.red;
    [SerializeField] Slider holdProgressBar;
    bool throwing = false;

    InputManager inputManager;

    Vector3 throwVelocity;
    Vector3 startPosition;
    List<Vector3> trajectoryPoints = new List<Vector3>();

    float holdDuration;
    float maxHoldDuration;
    float lineLength = 1f;
    float trajectoryHeight;
    float previousViewAngle = 0f;
    float lastCheckTime = 0f;
    float checkInterval = 0.1f;
    float adjustedValue;
    float easeOutPower = 3;

    void Start()
    {
        inputManager = InputManager.Instance;
        trajectoryHeight = minTrajectoryHeight;

        if (holdProgressBar != null)
        {
            holdProgressBar.minValue = 0;
            holdProgressBar.value = 0;
        }
    }

    void Update()
    {
        if (inputManager.IsLeftMouseButtonPressed())
        {
            InitializeThrow();
        }

        if (inputManager.IsLeftMouseButtonHeld())
        {
            UpdateThrow();
        }

        if (inputManager.IsLeftMouseButtonReleased())
        {
            ExecuteThrow();
        }
    }

    void InitializeThrow()
    {
        startPosition = transform.position;
        throwing = true;
        holdDuration = 0;
        lineLength = 0.01f;
        trajectoryPoints.Clear();

        OnThrowing?.Invoke(this, throwing);
    }

    void UpdateThrow()
    {
        Vector3 mouseWorldPos = MouseWorldPosition.GetMouseWorldPosition();
        maxHoldDuration = Mathf.Min(Vector3.Distance(startPosition, mouseWorldPos), maxLineLength) / lineGrowthRate;

        if (mouseWorldPos == Vector3.zero)
        {
            lineLength = AdjustValueBasedOnCameraAngle(lineLength, minLineLength, maxLineLength);

            if (holdDuration <= maxHoldDuration + 2f)
            {
                easeOutPower = 4;

                float easedDuration = EaseOut(holdDuration / maxHoldDuration, easeOutPower);
                lineLength += Mathf.Clamp(Mathf.Min(easedDuration * lineGrowthRate, maxLineLength), minLineLength, maxLineLength);
            }
        }
        else
        {
            easeOutPower = 3;

            lineLength = Vector3.Distance(startPosition, mouseWorldPos);
            lineLength = Mathf.Min(holdDuration * lineGrowthRate, lineLength);
            lineLength = Mathf.Min(lineLength, maxLineLength);
        }

        holdDuration += Time.deltaTime;
        OnThrowStrenghtChange?.Invoke(this, lineLength);

        if (holdProgressBar != null)
        {
            holdProgressBar.maxValue = maxHoldDuration;
            holdProgressBar.value = Mathf.Clamp(EaseOut(holdDuration / maxHoldDuration, easeOutPower) * maxHoldDuration, 0, maxHoldDuration);
        }
    }

    void ExecuteThrow()
    {
        Vector3 endPoint = startPosition + orientation.forward * lineLength;
        throwVelocity = CalculateThrowVelocity(startPosition, endPoint, trajectoryHeight);
        throwing = false;

        OnThrowStrenghtChange?.Invoke(this, lineLength);

        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0;
        }

        GenerateTrajectoryPoints();

        adjustedValue = 1f;
        OnThrowing?.Invoke(this, throwing);
    }

    float AdjustValueBasedOnCameraAngle(float value, float min, float max)
    {
        if (Time.time - lastCheckTime >= checkInterval)
        {
            float viewAngle = cameraHolder.eulerAngles.x;
            float angleDifference = viewAngle - previousViewAngle;

            if (angleDifference > 180)
            {
                angleDifference -= 360;
            }
            else if (angleDifference < -180)
            {
                angleDifference += 360;
            }

            bool isIncreasing = angleDifference > 0.01f;

            if (angleDifference > 0.05f || angleDifference < -0.05f)
            {
                float adjustment = isIncreasing ? -0.40f : 0.25f;
                adjustedValue = Mathf.Clamp(adjustedValue + adjustment, min, max);
            }

            previousViewAngle = viewAngle;
            lastCheckTime = Time.time;
            value = adjustedValue;

            return adjustedValue;
        }

        value = adjustedValue;

        return adjustedValue;
    }

    void GenerateTrajectoryPoints()
    {
        trajectoryPoints.Clear();
        Vector3 position = transform.position + Vector3.up * 0.5f;
        Vector3 velocity = throwVelocity;
        float timeStep = 0.1f;

        for (float t = 0; t < 5f; t += timeStep)
        {
            Vector3 nextPosition = position + velocity * timeStep;
            velocity += Physics.gravity * timeStep;
            trajectoryPoints.Add(position);
            position = nextPosition;

            if (Physics.Raycast(trajectoryPoints[trajectoryPoints.Count - 1], nextPosition - trajectoryPoints[trajectoryPoints.Count - 1],
                out RaycastHit hit, (nextPosition - trajectoryPoints[trajectoryPoints.Count - 1]).magnitude))
            {
                trajectoryPoints.Add(hit.point);
                hitVisual.position = hit.point;
                break;
            }
        }
    }

    Vector3 CalculateThrowVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    float EaseOut(float t, float easeOutPow)
    {
        return 1 - Mathf.Pow(1 - t, easeOutPow);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Vector3 startPosition = transform.position + Vector3.up * 0.5f;
        Vector3 endPosition = startPosition + orientation.forward * lineLength;
        Gizmos.DrawLine(startPosition, endPosition);

        if (!throwing)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < trajectoryPoints.Count - 1; i++)
            {
                Gizmos.DrawLine(trajectoryPoints[i], trajectoryPoints[i + 1]);
            }
        }
    }
}
