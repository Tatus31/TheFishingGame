using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    PlayerMovement player;
    float maxSpeed;
    float accelAmount;
    float frictionAmount;

    float footstepBlockDuration = 1f;

    Vector3 lastContactNormal;

    public WalkState(PlayerMovement player, float maxSpeed, float accelAmount, float frictionAmount)
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
        lastContactNormal = contactNormal;
    }

    public override void UpdateState()
    {
        if (player.inputManager.IsHoldingSprintKey())
        {
            player.SwitchState(player.SprintState);
        }
    }

    public override void FixedUpdateState()
    {
        UpdateState(player);
        Move(player, player.maxSpeedTime, maxSpeed, accelAmount);
        ApplyFriction(player, frictionAmount);

        HandleFootstepAudio();
    }


    private void HandleFootstepAudio()
    {
        bool isSwimming = PlayerMovement.Instance.IsSwimming;
        bool isStill = player.rb.velocity.magnitude < 0.1f;
        bool isControllingShip = StickToShip.Instance.IsControllingShip;

        if (isSwimming || isStill || isControllingShip)
        {
            AudioManager.MuteSound(AudioManager.WalkSound);
            return;
        }

        bool enoughTimePassedSinceWater = (Time.time - player.LastExitWaterTime) > footstepBlockDuration;

        if (OnGround && enoughTimePassedSinceWater)
        {
            AudioManager.UnmuteSound(AudioManager.WalkSound);
        }
        else
        {
            AudioManager.MuteSound(AudioManager.WalkSound);
        }
    }


    public override void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        base.ApplyFriction(player, frictionAmount);
    }

    public override void Move(PlayerMovement player, float maxSpeedTime, float maxSpeed, float accelAmount)
    {
        base.Move(player, maxSpeedTime, maxSpeed, accelAmount);
    }

    protected override void CheckGroundContacts(PlayerMovement player)
    {
        base.CheckGroundContacts(player);
    }

    public override void PlayAnimation(PlayerMovement player)
    {

    }

    public override void DrawGizmos(PlayerMovement player)
    {
        base.DrawGizmos(player);
    }
}
