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
        CharacterAnimator,
        LureAnimator
    }

    [Serializable]
    public class AnimatorEntry
    {
        public Animators animatorType;
        public Animator animator;
    }

    [SerializeField]
    List<AnimatorEntry> animators = new List<AnimatorEntry>();

    Dictionary<Animators, Animator> animatorDictionary = new Dictionary<Animators, Animator>();

    [HideInInspector] public static string ON_RUN = "onRun";
    [HideInInspector] public static string ON_THROW = "onThrow";
    [HideInInspector] public static string FLEE_LEFT = "fleeLeft";
    [HideInInspector] public static string FLEE_RIGHT = "fleeRight";
    [HideInInspector] public static string REEL = "reel";
    [HideInInspector] public static string DONE_FISHING = "doneFishing";
    [HideInInspector] public static string FISH_FLEEING = "fishFleeing";
    [HideInInspector] public static string LURE_CATCH = "lureCatch";

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there already is a {Instance.name} in the scene");

        Instance = this;

        foreach (var entry in animators)
        {
            if (entry.animator != null && !animatorDictionary.ContainsKey(entry.animatorType))
            {
                animatorDictionary.Add(entry.animatorType, entry.animator);
            }
        }
    }

    public Animator GetAnimator(Animators animatorType)
    {
        if (animatorDictionary.TryGetValue(animatorType, out Animator animator))
        {
            return animator;
        }
        else
        {
            Debug.LogWarning($"Animator of type {animatorType} not found!");
            return null;
        }
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
