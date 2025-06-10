using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RepairMiniGame : MonoBehaviour
{
    [SerializeField] bool isFixingHole = false;
    [SerializeField] bool isFixed = false;

    [SerializeField] float maxPointDistance = 10f;
    [SerializeField] float repairTimeMultiplier = 0.5f;

    [SerializeField] LayerMask repairPointLayerMask;

    [SerializeField] CameraLook cameraLook;

    Animator repairAnimator;

    bool wasFixingLastFrame = false;

    private void Start()
    {
        repairAnimator = AnimationController.Instance.GetAnimator(AnimationController.Animators.RepairMiniGameAnimator);
        repairAnimator.SetFloat(AnimationController.FIX_SPEED_MULTIPLIER, repairTimeMultiplier);

        repairAnimator.TryGetComponent(out CanvasGroup canvasGroup);
    }

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonHeld())
        {
            isFixingHole = true;
            repairAnimator.SetBool(AnimationController.IS_FIXING_HOLE, isFixingHole);
        }
        else
        {
            isFixingHole = false;
            repairAnimator.SetBool(AnimationController.IS_FIXING_HOLE, isFixingHole);
        }

        CheckAnimationCompletion();

        wasFixingLastFrame = isFixingHole;
    }

    void CheckAnimationCompletion()
    {
        if (wasFixingLastFrame && !isFixingHole && !isFixed)
        {
            AnimatorStateInfo stateInfo = repairAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("FixHole") && stateInfo.normalizedTime >= 0.95f)
            {
                isFixed = true;
                OnRepairComplete();
            }
        }
    }

    void OnRepairComplete()
    {
#if UNITY_EDITOR
        Debug.Log("Repair completed!");
#endif

        GameObject repairPointObj = MouseWorldPosition.GetObjectOverMouse(maxPointDistance, repairPointLayerMask);
        if (repairPointObj != null)
        {
            Vector3 repairPointPosition = repairPointObj.transform.position;
            repairPointObj.SetActive(false);

            ShipRepairPoints shipRepairPoints = FindObjectOfType<ShipRepairPoints>();
            if (shipRepairPoints != null)
            {
                shipRepairPoints.ResetRepairPointAtPosition(repairPointPosition);
                isFixed = false;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cameraLook.Sensitivity = 1f;
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}