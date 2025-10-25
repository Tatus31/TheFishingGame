using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObjectsWithShip : MonoBehaviour
{
    [SerializeField]
    AnimationDataSO animationDataSO;
    [SerializeField]
    ShipMovement shipMovement;

    List<Tweener> moveTweeners = new List<Tweener>();
    List<Tweener> rotateTweeners = new List<Tweener>();
    bool tweensInitialized = false;
    float invMaxWheelRotationTimesTwo;
    float currentAnimationPosition = 0f;
    float animationVelocity = 0f;

    [Header("Animation Smoothing")]
    [SerializeField] float smoothTime = 0.1f;

    public AnimationDataSO AnimationDataSO { get { return animationDataSO; } }

    private void Start()
    {
        if (shipMovement == null)
        {
            shipMovement = GetComponentInParent<ShipMovement>();
        }

        float maxWheelRotation = shipMovement.MaxWheelRotation;
        invMaxWheelRotationTimesTwo = 1f / (2f * maxWheelRotation);

        InitializeTweens();
    }

    private void InitializeTweens()
    {
        if (animationDataSO == null || animationDataSO.AnimationData == null)
            return;

        foreach (var animationData in animationDataSO.AnimationData)
        {
            Tweener moveTween = transform.DOLocalMove(animationData.MoveToPosition, animationData.MoveDuration).SetEase(animationData.MoveEase).SetAutoKill(false).Pause();
            moveTweeners.Add(moveTween);

            Tweener rotateTween = transform.DOLocalRotateQuaternion(animationData.RotateToRotation, animationData.RotateDuration).SetEase(animationData.RotateEase).SetAutoKill(false).Pause();
            rotateTweeners.Add(rotateTween);
        }

        tweensInitialized = true;
    }

    private void Update()
    {
        if (!tweensInitialized || shipMovement == null)
            return;

        UpdateAnimationBasedOnWheelRotation();
    }

    private void UpdateAnimationBasedOnWheelRotation()
    {
        float currentWheelRotation = shipMovement.CurrentWheelRotation;
        float maxWheelRotation = shipMovement.MaxWheelRotation;

        float targetNormalizedRotation = (currentWheelRotation + maxWheelRotation) * invMaxWheelRotationTimesTwo;
        targetNormalizedRotation = Mathf.Clamp01(targetNormalizedRotation);

        currentAnimationPosition = Mathf.SmoothDamp(currentAnimationPosition, targetNormalizedRotation, ref animationVelocity,smoothTime);

        foreach (var moveTween in moveTweeners)
        {
            if (moveTween != null && moveTween.IsActive())
            {
                moveTween.fullPosition = currentAnimationPosition;
            }
        }

        foreach (var rotateTween in rotateTweeners)
        {
            if (rotateTween != null && rotateTween.IsActive())
            {
                rotateTween.fullPosition = currentAnimationPosition;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var tween in moveTweeners)
        {
            if (tween != null)
                tween.Kill();
        }

        foreach (var tween in rotateTweeners)
        {
            if (tween != null)
                tween.Kill();
        }

        moveTweeners.Clear();
        rotateTweeners.Clear();
    }
}