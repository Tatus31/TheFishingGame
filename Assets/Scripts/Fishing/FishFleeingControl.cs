using Cinemachine;
using System;
using TMPro;
using UnityEngine;

public class FishFleeingControl : MonoBehaviour
{
    public static FishFleeingControl Instance;

    [SerializeField] Transform fishObject;

    [SerializeField] TextMeshProUGUI angleToLeftText;
    [SerializeField] TextMeshProUGUI angleToRightText;

    [SerializeField] float correctionStrength = 0.9f;

    FishingStateManager fishingStateManager;
    InputManager inputManager;

    float mouseInput;
    float fishDirection;
    bool onFleeing;

    Flee.FleeDirection currentFleeDirection;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fishingStateManager = FindObjectOfType<FishingStateManager>();
        fishingStateManager.fleeState.OnFleeingFish += fishingStateManager_OnFleeingFish;
        fishingStateManager.fleeState.OnCurrentFleeDirection += fishingStateManager_OnCurrentFleeDirection;

        inputManager = InputManager.Instance;
    }


    void Update()
    {
        if (onFleeing)
        {
            CounterFlee();
        }
    }

    private void fishingStateManager_OnFleeingFish(object sender, bool e)
    {
        onFleeing = e;
    }

    private void fishingStateManager_OnCurrentFleeDirection(object sender, Flee.FleeDirection e)
    {
        currentFleeDirection = e;
        fishDirection = currentFleeDirection == Flee.FleeDirection.Right ? 1 : -1;
    }


    void CounterFlee()
    {
        mouseInput = inputManager.GetMouseDelta().x;

        angleToLeftText.text = fishDirection.ToString("F1");
        angleToRightText.text = mouseInput.ToString("F1");

        if ((fishDirection > 0 && mouseInput < 0) || (fishDirection < 0 && mouseInput > 0))
        {
            fishingStateManager.fleeState.ReduceFleeProgress(correctionStrength * Time.deltaTime);
            //Debug.Log("countered flee");
        }
        else
        {
            //Debug.Log("Fish escaping");
        }
    }
}
