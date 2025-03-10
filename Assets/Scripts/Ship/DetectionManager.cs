using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionManager : MonoBehaviour
{
    public static event EventHandler OnDetectionChange;

    [Serializable]
    public enum DangerState
    {
        Low,
        Medium,
        High,
        Extreme
    }

    [SerializeField] DangerState currentState;
    [SerializeField] float timer;

    float time = 0;

    private void Start()
    {
        currentState = DangerState.Low;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if(time >= timer)
        {
            currentState++;
            time = 0;
            OnDetectionChange?.Invoke(this, EventArgs.Empty);
        }

        //Testing

        if(currentState == DangerState.Medium)
        {
            MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.StalkingState);
        }
        if (currentState == DangerState.Extreme)
        {
            MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.AttackingState);
            currentState = DangerState.Low;
        }
    }
}
