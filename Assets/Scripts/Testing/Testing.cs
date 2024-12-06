using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public static Testing Instance;

    public event EventHandler<bool> OnLookingBehind;

    [SerializeField] Transform testingSphere;
    [SerializeField] Transform orientation;
    [SerializeField] Transform fishObject;

    FishingStateManager fishingStateManager;

    bool onFleeing;
    bool onReeledIn;
    bool directionsSet;
    bool isLookingRight;
    float rotationSpeed = 5f; 

    Vector3 lineDirectionLeft;
    Vector3 lineDirectionRight;

    float behindAngleThreshold = 30f;
    //float gizmoLineLength = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        fishingStateManager = FindObjectOfType<FishingStateManager>();

        fishingStateManager.fleeState.OnFleeingFish += fishingStateManager_OnFleeingFish;
        fishingStateManager.reelState.OnReeledIn += fishingStateManager_OnReeledIn;
    }

    void Update()
    {
        testingSphere.position = MouseWorldPosition.GetMouseWorldPosition();


        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 direction = fishObject.position - fishingStateManager.GetCinemachineVirtualCamera().transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            fishingStateManager.GetCinemachineVirtualCamera().transform.rotation = Quaternion.Slerp(fishingStateManager.GetCinemachineVirtualCamera().transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.L))
        {
            fishingStateManager.GetCinemachineVirtualCamera().LookAt = fishObject;
        }

        if (Input.GetKey(KeyCode.U))
        {
            fishingStateManager.GetCinemachineVirtualCamera().LookAt = null;
        }

        //if (onFleeing)
        //{
        //    CheckIfLookingBehind();
        //}
    }

    private void fishingStateManager_OnFleeingFish(object sender, bool e)
    {
        onFleeing = e;

        if (onFleeing && !directionsSet)
        {
            lineDirectionLeft = -orientation.right;
            lineDirectionRight = orientation.right;

            directionsSet = true;
        }
    }

    private void fishingStateManager_OnReeledIn(object sender, bool e)
    {
        onReeledIn = e;

        if (onReeledIn)
        {
            directionsSet = false;
        }
    }

    void CheckIfLookingBehind()
    {
        float angleToLeft = Vector3.SignedAngle(-orientation.forward, lineDirectionLeft, Vector3.up);
        float angleToRight = Vector3.SignedAngle(-orientation.forward, lineDirectionRight, Vector3.up);

        if (angleToLeft <= 5f && angleToLeft >= -behindAngleThreshold)
        {
            if(fishingStateManager.fleeState.CurrentFleeDirection == Flee.FleeDirection.Left)
            {
                isLookingRight = true;

                OnLookingBehind?.Invoke(this, isLookingRight);
                Debug.Log("looking behind the right line");
            }
        }
        else if (angleToRight >= -5f && angleToRight <= behindAngleThreshold)
        {
            if (fishingStateManager.fleeState.CurrentFleeDirection == Flee.FleeDirection.Right)
            {
                isLookingRight = true;

                OnLookingBehind?.Invoke(this, isLookingRight);
                Debug.Log("looking behind the left line");
            }
        }
        else
        {
            Debug.Log("not looking directly behind either line");
        }

    }

    //private void OnDrawGizmos()
    //{
    //    if (!onFleeing) return;

    //    Gizmos.color = Color.yellow;

    //    Vector3 startPosition = orientation.position + Vector3.up * 0.5f;

    //    if (fishingStateManager.fleeState.CurrentFleeDirection == Flee.FleeDirection.Right)
    //    {
    //        Vector3 leftEndPosition = startPosition + lineDirectionLeft * gizmoLineLength;
    //        Gizmos.DrawLine(startPosition, leftEndPosition);

    //        Vector3 leftThresholdDirection = Quaternion.Euler(0, -behindAngleThreshold, 0) * lineDirectionLeft;

    //        Gizmos.DrawLine(startPosition, startPosition + leftThresholdDirection * gizmoLineLength);
    //        Gizmos.DrawLine(startPosition + leftThresholdDirection * gizmoLineLength, startPosition + lineDirectionLeft * gizmoLineLength);

    //        Gizmos.color = Color.red;
    //    }
    //    else
    //    {
    //        Vector3 rightEndPosition = startPosition + lineDirectionRight * gizmoLineLength;
    //        Gizmos.DrawLine(startPosition, rightEndPosition);

    //        Vector3 rightThresholdDirection = Quaternion.Euler(0, behindAngleThreshold, 0) * lineDirectionRight;

    //        Gizmos.DrawLine(startPosition, startPosition + rightThresholdDirection * gizmoLineLength);
    //        Gizmos.DrawLine(startPosition + rightThresholdDirection * gizmoLineLength, startPosition + lineDirectionRight * gizmoLineLength);

    //        Gizmos.color = Color.blue;
    //    }

    //}
}
