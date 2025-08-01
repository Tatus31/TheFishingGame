using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitSprintState : MovementBaseState
{
    private PlayerMovement player;
    private float maxSpeed;
    private float accelAmount;
    private float frictionAmount;

    public SuitSprintState(PlayerMovement player, float maxSpeed, float accelAmount, float frictionAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
        this.frictionAmount = frictionAmount;
    }

    public override void EnterState(PlayerMovement player)
    {
        base.EnterState(player);
    }

    public override void ExitState()
    {
    }


    public override void FixedUpdateState()
    {
        Move(player, player.maxSpeedTime, maxSpeed, accelAmount);
        ApplyFriction(player, frictionAmount);
    }

    public override void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        base.ApplyFriction(player, frictionAmount);
    }

    public override void Move(PlayerMovement player, float maxSpeedTime, float maxSpeed, float accelAmount)
    {
        base.Move(player, maxSpeedTime, maxSpeed, accelAmount);
    }

    public override void PlayAnimation(PlayerMovement player)
    {
        //if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.DivingSuit))
        //    player.GetAnimationController().PlayAnimation(player.GetFreeHandAnimator(), AnimationController.ON_RUN, true);
        //if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.FishingRod))
        //    player.GetAnimationController().PlayAnimation(player.GetFishingAnimator(), AnimationController.ON_RUN, true);
    }

    public override void UpdateState()
    {

    }
}
