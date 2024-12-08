using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Flee : FishingBaseState
{
    //TODO: Rework this script

    public event EventHandler<bool> OnFleeingFish;

    Transform fishObject;
    Image pullCheck;

    Vector3 fleeStartPosition;
    Vector3 fleeTargetPosition;

    float reelInTimer = 0;
    float reelInTime = 1f;

    float minFleeTime = 2f;
    float maxFleeTime = 5f;
    float fleeTimer;

    bool isFleeing;

    Vector3 originalPosition;
    //bool isLookingBehind;

    public enum FleeDirection { Left, Right }
    public FleeDirection CurrentFleeDirection { get; private set; }

    public void Initialize(Transform fishObject, Image pullCheck, float reelInTime)
    {
        this.fishObject = fishObject;
        this.pullCheck = pullCheck;
        this.reelInTime = reelInTime;
    }

    public override void EnterState(FishingStateManager fishingState)
    {
        isFleeing = true;
        OnFleeingFish?.Invoke(this, isFleeing);

        pullCheck.color = Color.red;

        fleeStartPosition = fishObject.position; 
        originalPosition = fishingState.GetStartFishPosition();

        int directionChoice = Random.Range(0, 2);
        CurrentFleeDirection = directionChoice == 0 ? FleeDirection.Left : FleeDirection.Right;

        float horizontalOffset = (CurrentFleeDirection == FleeDirection.Left) ? -1.0f : 1.0f;
        fleeTargetPosition = originalPosition + new Vector3(0, 0, horizontalOffset); 

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

        if (fleeTimer <= 0)
        {
            isFleeing = false;
            //isLookingBehind = false;

            OnFleeingFish?.Invoke(this, isFleeing);
            fishingState.SwitchState(fishingState.reelState);
            return;
        }

        //if (isLookingBehind)
        //{
        //    Debug.Log("holding");

        //    return;
        //}

        reelInTimer += Time.deltaTime;
        float t = Mathf.Clamp01(reelInTimer / reelInTime);
        fishObject.position = Vector3.Lerp(fleeStartPosition, fleeTargetPosition, t);
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


    public override void DrawGizmos(FishingStateManager fishingState)
    {
        Gizmos.color = Color.black;

#if UNITY_EDITOR
        Handles.DrawLine(fishObject.position, fleeTargetPosition, 3f);
#endif
        Gizmos.DrawSphere(fleeTargetPosition, 0.1f);
    }
}
