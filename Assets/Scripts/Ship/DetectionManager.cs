using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipMovement;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance;

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
    [SerializeField] float timer = 5f;

    float time;
    float currentDetectionMultiplier;
    SpeedLevel speedLevel;

    bool isSearching = false;

    public float CurrentDetectionMultiplier { get { return currentDetectionMultiplier; } set { currentDetectionMultiplier = value; } }

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;
    }

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

        if (currentDetectionMultiplier >= 1.9f)
        {
            if (MonsterStateMachine.Instance == null)
                return;

            time += Time.deltaTime;

            if (time >= timer)
            {
                isSearching = false;
                time = 0;
            }

            if (!isSearching)
            {
                MonsterStateMachine.Instance.InvestigatingState.SetInvestigationRadius(currentDetectionMultiplier);
                MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.InvestigatingState);
                isSearching = true;
            }
        }

        //if (shipIsNotMoving)
        //{

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
        currentDetectionMultiplier = Mathf.Round(currentDetectionMultiplier * 1000f) / 1000f;

#if UNITY_EDITOR
        Debug.Log($"current detection: {currentDetectionMultiplier} current speed level: {speedLevel}");
#endif
    }
}
