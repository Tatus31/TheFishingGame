using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController: MonoBehaviour
{
    public static AnimationController Instance;

    [Serializable]
    public enum Animators
    {
        FishingAnimator,
        LureAnimator,
        EmptyHandsAnimator,
        HarpoonAnimator,
        RepairMiniGameAnimator,
        ShipLampAnimator,
        ShipGearAnimator,
        ShipWheelAnimator,
    }

    [Serializable]
    public class AnimatorEntry
    {
        public Animators animatorType;
        public Animator animator;
    }

    [SerializeField]
    List<AnimatorEntry> animators = new List<AnimatorEntry>();

    [HideInInspector] public const string ON_RUN = "onRun";
    [HideInInspector] public const string ON_THROW = "onThrow";
    [HideInInspector] public const string FLEE_LEFT = "fleeLeft";
    [HideInInspector] public const string FLEE_RIGHT = "fleeRight";
    [HideInInspector] public const string REEL = "reel";
    [HideInInspector] public const string DONE_FISHING = "doneFishing";
    [HideInInspector] public const string FISH_FLEEING = "fishFleeing";
    [HideInInspector] public const string LURE_CATCH = "lureCatch";
    [HideInInspector] public const string HARPOON_AIM = "harpoonAim";
    [HideInInspector] public const string ON_HIT = "onHit";
    [HideInInspector] public const string SPEED_MULTIPLIER = "speedMultiplier";
    [HideInInspector] public const string GEAR_STATE = "gearState";
    [HideInInspector] public const string WHEEL_BLEND_STATE = "WheelRotation";
    [HideInInspector] public const string IS_FIXING_HOLE = "isFixingHole";
    [HideInInspector] public const string FIX_SPEED_MULTIPLIER = "fixSpeedMultiplier";

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there already is a {Instance.name} in the scene");

        Instance = this;
    }

    public Animator GetAnimator(Animators animatorType)
    {

        foreach (var animator in animators)
        {
            if (animator.animatorType == animatorType)
            {
                return animator.animator;
            }
        }

        Debug.LogError($"there is no animator {animatorType}");
        return null;
    }

    public void PlayAnimation<T>(Animator animator, string parameterName, T value)
    {
        if (animator == null)
        {
            Debug.LogError("Cannot play animation no aniamtor");
            return;
        }

        switch (value)
        {
            case bool boolValue:
                animator.SetBool(parameterName, boolValue);
                break;

            case float floatValue:
                animator.SetFloat(parameterName, floatValue);
                break;

            case int intValue:
                animator.SetInteger(parameterName, intValue);
                break;

            case string stringValue when stringValue.Equals("trigger"):
                animator.SetTrigger(parameterName);
                break;

            default:
                Debug.LogError($"no {typeof(T)} as a animator parameter");
                break;
        }
    }
}
