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

    [SerializeField] float fishStrengthOffset = 0;

    FishingStateManager fishingStateManager;
    InputManager inputManager;

    float mouseInput;
    float fishDirection;
    bool onFleeing;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fishingStateManager = FindObjectOfType<FishingStateManager>();
        fishingStateManager.fleeState.OnFleeingFish += fishingStateManager_OnFleeingFish;

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

    void CounterFlee()
    {
        mouseInput = inputManager.GetMouseDelta().x;

        angleToLeftText.text = fishDirection.ToString("F1");
        angleToRightText.text = mouseInput.ToString("F1");

        fishDirection = fishingStateManager.fleeState.CurrentFleeDirection == Flee.FleeDirection.Right ? 1 : -1;       

        if ((fishDirection > 0 && mouseInput < fishStrengthOffset) || (fishDirection < 0 && mouseInput > fishStrengthOffset))
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
