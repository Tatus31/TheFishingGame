using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;
using static UnityEngine.ParticleSystem;

public class ToolBoxState : MovementBaseState
{
    

    public PlayerMovement player;
    float maxSpeed, accelAmount;
    public ParticleSystem particle;
    public Camera FPCamera;
    Player Player;
    ItemObject item;

    

    public ToolBoxState(PlayerMovement player, float maxSpeed, float accelAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
    }


    public override void EnterState(PlayerMovement player)
    {
        Debug.Log("ToolBox State");
    }

    public override void ExitState()
    {
        
    }


    public override void FixedUpdateState()
    {
        base.Move(player, maxSpeed, maxSpeed, accelAmount);
        ApplyFriction(player, 1);
    }

    public override void PlayAnimation(PlayerMovement player)
    {
        
    }

    public override void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        base.ApplyFriction(player, frictionAmount);
    }

    public override void UpdateState()
    {

    }
}    
