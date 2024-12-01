using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI angleToLeftText;
    [SerializeField] TextMeshProUGUI angleToRightText;

    [SerializeField] Transform testingSphere;
    [SerializeField] Transform orientation;

    FishingStateManager fishingStateManager;

    bool onThrowing;
    Vector3 lineDirectionLeft;
    Vector3 lineDirectionRight;

    float behindAngleThreshold = 30f;
    float gizmoLineLength = 2f;

    private void Start()
    {
        fishingStateManager = FindObjectOfType<FishingStateManager>();
        fishingStateManager.throwState.OnThrowing += Throw_OnThrowing;
    }

    void Update()
    {
        testingSphere.position = MouseWorldPosition.GetMouseWorldPosition();

        if (onThrowing)
        {
            CheckIfLookingBehind();
        }
    }

    private void Throw_OnThrowing(object sender, bool e)
    {
        onThrowing = e;

        if (onThrowing)
        {
            lineDirectionLeft = -orientation.right;
            lineDirectionRight = orientation.right;
        }
    }

    void CheckIfLookingBehind()
    {
        float angleToLeft = Vector3.SignedAngle(-orientation.forward, lineDirectionLeft, Vector3.up);
        float angleToRight = Vector3.SignedAngle(-orientation.forward, lineDirectionRight, Vector3.up);

        angleToLeftText.text = angleToLeft.ToString("F1");
        angleToRightText.text = angleToRight.ToString("F1");

        if (angleToLeft <= 5f && angleToLeft >= -behindAngleThreshold)
        {
            Debug.Log("looking behind the right line");
        }
        else if (angleToRight >= -5f && angleToRight <= behindAngleThreshold)
        {
            Debug.Log("looking behind the left line");
        }
        else
        {
            Debug.Log("not looking directly behind either line");
        }

    }

    private void OnDrawGizmos()
    {
        if (onThrowing)
        {
            Gizmos.color = Color.yellow;

            Vector3 startPosition = orientation.position + Vector3.up * 0.5f;

            Vector3 leftEndPosition = startPosition + lineDirectionLeft * gizmoLineLength;
            Vector3 rightEndPosition = startPosition + lineDirectionRight * gizmoLineLength;

            Gizmos.DrawLine(startPosition, leftEndPosition);
            Gizmos.DrawLine(startPosition, rightEndPosition);

            Gizmos.color = Color.green;

            Vector3 leftThresholdDirection = Quaternion.Euler(0, -behindAngleThreshold, 0) * lineDirectionLeft;
            Vector3 rightThresholdDirection = Quaternion.Euler(0, behindAngleThreshold, 0) * lineDirectionRight;

            Gizmos.DrawLine(startPosition, startPosition + leftThresholdDirection * gizmoLineLength);
            Gizmos.DrawLine(startPosition, startPosition + rightThresholdDirection * gizmoLineLength);

            Gizmos.DrawLine(startPosition + leftThresholdDirection * gizmoLineLength, startPosition + lineDirectionLeft * gizmoLineLength);
            Gizmos.DrawLine(startPosition + rightThresholdDirection * gizmoLineLength, startPosition + lineDirectionRight * gizmoLineLength);
        }
    }
}
