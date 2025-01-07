using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : MovementBaseState
{
    private PlayerMovement player;
    private float maxSpeed;
    private float accelAmount;
    private float frictionAmount;

    public SprintState(PlayerMovement player, float maxSpeed, float accelAmount, float frictionAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
        this.frictionAmount = frictionAmount;
    }

    public override void EnterState(PlayerMovement player)
    {
        player.GetAnimationController().PlayAnimation(player.GetFishingAnimator(), AnimationController.ON_RUN, true);
        //Debug.Log("Entered Sprint State");
    }

    public override void ExitState()
    {
        //Debug.Log("Exited Sprint State");
    }

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        //Debug.Log("in sprint fixed");

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
}

