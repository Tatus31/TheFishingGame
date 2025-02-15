using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class FlippersState : MovementBaseState
{
    public PlayerMovement player;
    float maxSpeed;
    float accelAmount;
   

    public FlippersState(PlayerMovement player, float maxSpeed, float accelAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
    }

    public override void EnterState(PlayerMovement player)
    {
        Debug.Log("flipper state");
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        base.Move(player, maxSpeed, maxSpeed, accelAmount);
        ApplyFriction(player, 1);
    }
    public override void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        base.ApplyFriction(player, frictionAmount);
    }

    public override void PlayAnimation(PlayerMovement player)
    {

    }
}
