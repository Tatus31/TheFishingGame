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

    //[SerializeField] float correctionStrength = 0.9f;

    FishingStateManager fishingStateManager;
    InputManager inputManager;

    float mouseInput;
    float fishDirection;
    bool onFleeing;

    Flee.FleeDirection currentFleeDirection;

    void Awake()
    {
        Instance = this;
        fishingStateManager = FindObjectOfType<FishingStateManager>();
    }

    void Start()
    {

        if (fishingStateManager != null)
        {
            //fishingStateManager.fleeState.OnFleeingFish += fishingStateManager_OnFleeingFish;
            //fishingStateManager.fleeState.OnCurrentFleeDirection += fishingStateManager_OnCurrentFleeDirection;
        }
        else
        {
            Debug.LogError("fishingStateManager not created yet");
        }

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
        var animation = fishingStateManager.GetAnimationController();
        mouseInput = inputManager.GetMouseDelta().x;

        angleToLeftText.text = fishDirection.ToString("F1");
        angleToRightText.text = mouseInput.ToString("F1");

        if ((fishDirection > 0 && mouseInput < 0) || (fishDirection < 0 && mouseInput > 0))
        {
            //fishingStateManager.fleeState.ReduceFleeProgress(correctionStrength * Time.deltaTime);

            if (fishDirection > 0)
            {
                animation.PlayAnimation(fishingStateManager.GetFishingAnimator(), AnimationController.FLEE_RIGHT, true);
                animation.PlayAnimation(fishingStateManager.GetFishingAnimator(), AnimationController.FLEE_LEFT, false);
            }
            else
            {
                animation.PlayAnimation(fishingStateManager.GetFishingAnimator(), AnimationController.FLEE_RIGHT, false);
                animation.PlayAnimation(fishingStateManager.GetFishingAnimator(), AnimationController.FLEE_LEFT, true);
            }
            //Debug.Log("countered flee");
        }
        else
        {
            //TODO: change after making the reel animation
            //animation.PlayAnimation(AnimationController.DONE_FISHING, false);
            //Debug.Log("Fish escaping");
        }
    }
}
