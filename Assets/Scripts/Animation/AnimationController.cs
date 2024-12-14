using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class AnimationController: MonoBehaviour
{
    public static AnimationController Instance;

    [HideInInspector] public static string ON_RUN = "onRun";
    [HideInInspector] public static string ON_THROW = "onThrow";

    Animator animator;

    void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there already is a {Instance.name} in the scene");

        Instance = this;

        animator = GetComponent<Animator>();
    }

    public void PlayAnimation<T>(string parameterName, T value)
    {
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
                Debug.LogError($"there is no {value} type of parameter");
                break;
        }
    }
}
