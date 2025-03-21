using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipMovement;

public class DetectionManager : MonoBehaviour
{

    [Serializable]
    public enum DangerState
    {
        Low,
        Medium,
        High,
        Extreme
    }

    [Header("Detection Values")]
    [SerializeField] float detectionMultiplierValue;
    [SerializeField]
    [Tooltip("time to interpolate between current detection and the target one")] float lerpTime = 1f;

    float currentDetectionMultiplier;
    SpeedLevel speedLevel;

    private void Start()
    {
        currentDetectionMultiplier = 0;
        speedLevel = SpeedLevel.neutral;

        OnDetectionChange += ShipMovement_OnDetectionChange;
    }

    private void ShipMovement_OnDetectionChange(object sender, ShipMovement.SpeedLevel e)
    {
        speedLevel = e;

        //Debug.Log($"current detection: {currentDetectionMultiplier} for: {e}");
    }

    private void Update()
    {
        switch (speedLevel)
        {
            case SpeedLevel.reverse:
                LerpDetectionValue(0.5f);
                break;
            case SpeedLevel.neutral:
                LerpDetectionValue(0);
                break;
            case SpeedLevel.forward1:
                LerpDetectionValue(1);
                break;
            case SpeedLevel.forward2:
                LerpDetectionValue(2);
                break;
            case SpeedLevel.forward3:
                LerpDetectionValue(3);
                break;
            default:
                break;
        }

        if(currentDetectionMultiplier >= 1.9f)
        {
            if (MonsterStateMachine.Instance == null)
                return;

            MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.StalkingState);
        }

        //if (shipIsNotMoving)
        //{
        //    time += Time.deltaTime;
        //    currentDetectionMultiplier--;

        //    if (time >= timer)
        //    {
        //        time = 0;
        //    }
        //}

        //Testing

        //if (MonsterStateMachine.Instance == null)
        //    return;


        //if (currentState == DangerState.Extreme)
        //{
        //    MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.AttackingState);
        //    currentState = DangerState.Low;
        //}
    }

    void LerpDetectionValue(float targetDetectionValue)
    {
        float previousDetectionMultiplier = currentDetectionMultiplier;

        currentDetectionMultiplier = Mathf.Lerp(currentDetectionMultiplier, targetDetectionValue, Time.deltaTime * lerpTime);
        currentDetectionMultiplier = Mathf.Round(currentDetectionMultiplier * 10000f) / 10000f;

#if UNITY_EDITOR
        Debug.Log($"current detection: {currentDetectionMultiplier} current speed level: {speedLevel}");
#endif
    }
}
