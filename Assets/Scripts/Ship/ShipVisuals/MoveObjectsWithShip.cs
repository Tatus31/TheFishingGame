using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveObjectsWithShip : MonoBehaviour
{
    [Header("Refrances")]
    [SerializeField]
    AnimationDataSO animationDataSO;
    [SerializeField]
    ShipMovement shipMovement;
    [Header("Animation Smoothing")]
    [SerializeField] float smoothSpeed = 5f;
    //[SerializeField] float inputReleaseDelay = 0.2f;

    List<Tweener> moveTweeners = new List<Tweener>();
    List<Tweener> rotateTweeners = new List<Tweener>();

    bool tweensInitialized = false;
    float currentAnimationPosition = 0.5f;
    //float timeSinceLastInput = 0f;
    //float lastTargetPosition = 0.5f; 

    Vector3 originalLocalPosition;
    Quaternion originalLocalRotation;

    public AnimationDataSO AnimationDataSO { get { return animationDataSO; } }

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;

        if (shipMovement == null)
        {
            shipMovement = GetComponentInParent<ShipMovement>();
        }

        float maxWheelRotation = shipMovement.MaxWheelRotation;

        InitializeTweens();
    }

    private void InitializeTweens()
    {
        if (animationDataSO == null || animationDataSO.AnimationData == null)
            return;

        foreach (var animationData in animationDataSO.AnimationData)
        {
            Tweener moveTween = transform.DOLocalMove(animationData.MoveToPosition, animationData.MoveDuration).SetEase(animationData.MoveEase).SetAutoKill(false).Pause();
            moveTween.ChangeStartValue(originalLocalPosition);
            moveTweeners.Add(moveTween);

            Tweener rotateTween = transform.DOLocalRotateQuaternion(animationData.RotateToRotation, animationData.RotateDuration).SetEase(animationData.RotateEase).SetAutoKill(false).Pause();
            rotateTween.ChangeStartValue(originalLocalRotation);
            rotateTweeners.Add(rotateTween);
        }

        tweensInitialized = true;
    }

    private void Update()
    {
        if (!tweensInitialized || shipMovement == null)
            return;

        bool hasInput = IsWheelBeingControlled();

        if (hasInput)
        {
            //timeSinceLastInput = 0f;
            UpdateAnimationBasedOnPlayerInput();
        }
        else
        {
            //timeSinceLastInput += Time.deltaTime;
            //if (timeSinceLastInput < inputReleaseDelay)
            //{
            //    UpdateAnimationBasedOnPlayerInput();
            //}
            //else
            //{
            //    currentAnimationPosition = lastTargetPosition;
            //    ApplyAnimationPosition();
            //}
        }
    }

    private bool IsWheelBeingControlled()
    {
        if (!shipMovement.IsControllingShip)
            return false;

        Vector2 movementInput = InputManager.Instance.GetShipMovement();
        float turnInput = movementInput.x;

        return Mathf.Abs(turnInput) > 0.01f;
    }

    private void UpdateAnimationBasedOnPlayerInput()
    {
        Vector2 movementInput = InputManager.Instance.GetShipMovement();
        float turnInput = movementInput.x;

        float targetNormalizedPosition = Mathf.Clamp01(turnInput * 0.5f + 0.5f);

        //lastTargetPosition = targetNormalizedPosition;
        currentAnimationPosition = Mathf.MoveTowards(currentAnimationPosition, targetNormalizedPosition, smoothSpeed * Time.deltaTime);

        ApplyAnimationPosition();
    }

    private void ApplyAnimationPosition()
    {
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