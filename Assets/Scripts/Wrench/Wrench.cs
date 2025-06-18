using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Wrench : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (MouseWorldPosition.GetInteractable(layerMask) && InputManager.Instance.IsLeftMouseButtonPressed())
        {
            ShipDamage.Instance.RestoreHealth(50);
            AnimationController.Instance.PlayAnimation(animator, AnimationController.ON_USE, true);
        }
        else
        {
            AnimationController.Instance.PlayAnimation(animator, AnimationController.ON_USE, false);
        }
    }
}
