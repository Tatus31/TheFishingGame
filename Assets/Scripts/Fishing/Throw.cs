using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Throw : FishingBaseState
{
    public event EventHandler<float> OnThrowStrenghtChange;
    public event EventHandler<bool> OnThrowing;

    Transform orientation;
    Transform hitVisual;
    Slider holdProgressBar;

    InputManager inputManager;
    ThrowSettings settings;

    bool throwing = false;
    Vector3 throwVelocity;
    Vector3 startPosition;
    List<Vector3> trajectoryPoints = new List<Vector3>();

    float holdDuration;
    float maxHoldDuration;
    float lineLength = 1f;
    float trajectoryHeight;
    float easeOutPower = 3;

    public void Initialize(ThrowSettings throwSettings, Transform orientation, Transform hitVisual, Slider holdProgressBar)
    {
        this.settings = throwSettings;
        this.orientation = orientation;
        this.hitVisual = hitVisual;
        this.holdProgressBar = holdProgressBar;

        trajectoryHeight = settings.minTrajectoryHeight;
        inputManager = InputManager.Instance;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        inputManager = InputManager.Instance;
        trajectoryHeight = settings.minTrajectoryHeight;

        if (holdProgressBar != null)
        {
            holdProgressBar.minValue = 0;
            holdProgressBar.value = 0;
        }
    }

    public override void UpdateState(FishingStateManager fishingState)
    {
        if (!fishingState.IsCooldownOver() || MouseWorldPosition.GetMouseWorldPosition() == Vector3.zero)
        {
            return;
        }

        if (inputManager.IsLeftMouseButtonPressed())
        {
            StartThrow(fishingState);
        }

        if (inputManager.IsLeftMouseButtonHeld())
        {
            UpdateThrow();
        }

        if (inputManager.IsLeftMouseButtonReleased())
        {
            EndThrow(fishingState);
        }
    }

    void StartThrow(FishingStateManager fishingState)
    {
        throwing = true;
        holdDuration = 0;
        lineLength = 0.01f;
        startPosition = fishingState.GetCurrentTransform().transform.position + Vector3.up * 0.5f;
        trajectoryPoints.Clear();
        OnThrowing?.Invoke(this, throwing);
    }

    void UpdateThrow()
    {
        Vector3 mouseWorldPos = MouseWorldPosition.GetMouseWorldPosition();
        if (mouseWorldPos == Vector3.zero) return;

        maxHoldDuration = Mathf.Min(Vector3.Distance(startPosition, mouseWorldPos), settings.maxLineLength) / settings.lineGrowthRate;

        easeOutPower = 3;

        lineLength = Mathf.Min(holdDuration * settings.lineGrowthRate, Vector3.Distance(startPosition, mouseWorldPos));
        lineLength = Mathf.Clamp(lineLength, settings.minLineLength, settings.maxLineLength);

        holdDuration += Time.deltaTime;

        OnThrowStrenghtChange?.Invoke(this, lineLength);

        if (holdProgressBar != null)
        {
            holdProgressBar.maxValue = maxHoldDuration;
            holdProgressBar.value = Mathf.Clamp(EaseOut(holdDuration / maxHoldDuration, easeOutPower) * maxHoldDuration, 0, maxHoldDuration);
        }
    }

    void EndThrow(FishingStateManager fishingState)
    {
        Vector3 endPoint = startPosition + orientation.forward * lineLength;
        throwVelocity = CalculateThrowVelocity(startPosition, endPoint, trajectoryHeight);
        throwing = false;
        OnThrowStrenghtChange?.Invoke(this, lineLength);

        if (holdProgressBar != null)
        {
            holdProgressBar.value = 0;
        }

        if (!throwing)
        {
            fishingState.SwitchState(fishingState.catchState);
        }

        OnThrowing?.Invoke(this, throwing);
        GenerateTrajectoryPoints(fishingState);
    }

    void GenerateTrajectoryPoints(FishingStateManager fishingState)
    {
        trajectoryPoints.Clear();
        Vector3 position = fishingState.GetCurrentTransform().transform.position + Vector3.up * 0.5f;
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

    public override void DrawGizmos(FishingStateManager fishingState)
    {
        Gizmos.color = settings.lineColor;
        Vector3 startPosition = fishingState.GetCurrentTransform().transform.position + Vector3.up * 0.5f;
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


    //Might be usefull

    //Transform cameraHolder;

    //float previousViewAngle = 0f;
    //float lastCheckTime = 0f;
    //float checkInterval = 0.1f;
    //float adjustedValue;

    //float AdjustValueBasedOnCameraAngle(float value, float min, float max)
    //{
    //    if (Time.time - lastCheckTime >= checkInterval)
    //    {
    //        float viewAngle = cameraHolder.eulerAngles.x;
    //        float angleDifference = viewAngle - previousViewAngle;

    //        if (angleDifference > 180)
    //        {
    //            angleDifference -= 360;
    //        }
    //        else if (angleDifference < -180)
    //        {
    //            angleDifference += 360;
    //        }

    //        bool isIncreasing = angleDifference > 0.01f;

    //        if (angleDifference > 0.05f || angleDifference < -0.05f)
    //        {
    //            float adjustment = isIncreasing ? -0.40f : 0.25f;
    //            adjustedValue = Mathf.Clamp(adjustedValue + adjustment, min, max);
    //        }

    //        previousViewAngle = viewAngle;
    //        lastCheckTime = Time.time;
    //        value = adjustedValue;

    //        return adjustedValue;
    //    }

    //    value = adjustedValue;

    //    return adjustedValue;
    //}
}