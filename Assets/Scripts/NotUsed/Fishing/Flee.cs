using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Flee : FishingBaseState
{
    public event EventHandler<bool> OnFleeingFish;
    public event EventHandler<FleeDirection> OnCurrentFleeDirection;

    Transform fishObject;
    Image pullCheck;

    Vector3 fleeStartPosition;
    Vector3 fleeTargetPosition;

    float reelInTimer;
    float reelInTime = 2f;

    float minFleeTime = 2f;
    float maxFleeTime = 5f;
    float fleeTimer;

    bool isFleeing;

    Vector3 originalPosition;
    float fleeRadius = 1.75f;

    float fleeTimes = 3;
    List<FleeData> fleeDirections = new List<FleeData>();
    int currentFleeDirectionIndex = 0;

    //bool isLookingBehind;

    public enum FleeDirection { Left, Right }

    public void Initialize(Transform fishObject, Image pullCheck, float reelInTime, float minFleeTime, float maxFleeTime, float fleeRadius, float fleeTimes)
    {
        this.fishObject = fishObject;
        this.pullCheck = pullCheck;
        this.reelInTime = reelInTime;
        this.minFleeTime = minFleeTime;
        this.maxFleeTime = maxFleeTime;
        this.fleeRadius = fleeRadius;
        this.fleeTimes = fleeTimes;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        isFleeing = true;
        OnFleeingFish?.Invoke(this, isFleeing);

        //TODO: change after adding the reeling animation
        fishingState.GetAnimationController().PlayAnimation(fishingState.GetFishingAnimator(), AnimationController.FISH_FLEEING, true);

        pullCheck.color = Color.red;

        fleeStartPosition = fishObject.position;
        originalPosition = fishObject.position;

        DecideFleeDirections();

        ResetFleeTimer();
        reelInTimer = 0f;
    }

    //private void Testing_OnLookingBehind(object sender, bool e)
    //{
    //    isLookingBehind = e;
    //}

    public override void UpdateState(FishingStateManager fishingState)
    {
        fleeTimer -= Time.deltaTime;

        if (FishEscaped())
        {
            Debug.Log("the fish escaped");
            fishingState.SwitchState(fishingState.escapedState);
            return;
        }

        if (fleeTimer <= 0)
        {
            currentFleeDirectionIndex++;

            if (currentFleeDirectionIndex < fleeDirections.Count)
            {
                fleeStartPosition = fishObject.position;
                fleeTargetPosition = GetFleeTargetByIndex(currentFleeDirectionIndex);

                ResetFleeTimer();
                reelInTimer = 0f;
            }
            else
            {
                isFleeing = false;
                OnFleeingFish?.Invoke(this, isFleeing);
                fishingState.GetAnimationController().PlayAnimation(fishingState.GetFishingAnimator(), AnimationController.FISH_FLEEING, false);
                fishingState.SwitchState(fishingState.reelState);
                return;
            }
        }


        float fleeSpeedMultiplier = 0.5f;
        reelInTimer += Time.deltaTime * fleeSpeedMultiplier;
        float t = Mathf.Clamp01(reelInTimer / reelInTime);
        fishObject.position = Vector3.Lerp(fleeStartPosition, fleeTargetPosition, t);

        Vector3 directionToTarget = (fleeTargetPosition - fishObject.position).normalized;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            fishObject.rotation = Quaternion.Slerp(fishObject.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void ResetFleeTimer()
    {
        fleeTimer = Random.Range(minFleeTime, maxFleeTime);
    }

    public void ReduceFleeProgress(float reductionAmount)
    {
        reelInTimer -= reductionAmount;

        reelInTimer = Mathf.Clamp(reelInTimer, 0f, reelInTime);

        float t = reelInTimer / reelInTime;
        fishObject.position = Vector3.Lerp(fleeStartPosition, fleeTargetPosition, t);
    }

    bool FishEscaped()
    {
        Bounds escapeBounds = new Bounds(fleeStartPosition, new Vector3(fleeRadius * 2, fleeRadius * 2, fleeRadius * 2));
        return !escapeBounds.Contains(fishObject.position);
    }

    void DecideFleeDirections()
    {
        fleeDirections.Clear();
        currentFleeDirectionIndex = 0;

        Vector3 currentPosition = originalPosition;

        for (int i = 0; i < fleeTimes; i++)
        {
            FleeDirection direction = (Random.Range(0, 2) == 0) ? FleeDirection.Left : FleeDirection.Right;
            float horizontalOffset = (direction == FleeDirection.Left) ? -3f : 3f;

            Vector3 newFleeTarget = currentPosition + new Vector3(-1f, 0, horizontalOffset);

            fleeDirections.Add(new FleeData(newFleeTarget, direction));
            currentPosition = newFleeTarget;
        }

        fleeTargetPosition = GetFleeTargetByIndex(currentFleeDirectionIndex);
    }

    Vector3 GetFleeTargetByIndex(int index)
    {
        if (index < fleeDirections.Count)
        {
            OnCurrentFleeDirection?.Invoke(this, fleeDirections[index].Direction);
            return fleeDirections[index].Position;
        }

        return Vector3.zero;
    }

    public override void DrawGizmos(FishingStateManager fishingState)
    {
        Gizmos.color = Color.black;

#if UNITY_EDITOR
        Handles.DrawLine(fishObject.position, fleeTargetPosition, 3f);
#endif
        Gizmos.DrawSphere(fleeTargetPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fleeStartPosition, fleeRadius);
        Gizmos.DrawCube(fleeStartPosition, Vector3.one * 0.1f);
    }
}
