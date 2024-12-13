using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : IMovementState
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

    public void EnterState(PlayerMovement player)
    {
        player.GetAnimationController().PlayAnimation(player.GetAnimationController().ON_RUN, true);
        //Debug.Log("Entered Sprint State");
    }

    public void ExitState()
    {
        //Debug.Log("Exited Sprint State");
    }

    public void UpdateState() { }

    public void FixedUpdateState()
    {
        //Debug.Log("in sprint fixed");

        Move(player.maxSpeedTime);
        ApplyFriction();
    }

    void ApplyFriction()
    {
        if (player.GetMoveDirection().magnitude < 0.01f)
        {
            Vector3 horizontalVelocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);
            if (horizontalVelocity.magnitude > 0)
            {
                float friction = Mathf.Min(horizontalVelocity.magnitude, frictionAmount);
                Vector3 frictionForce = -horizontalVelocity.normalized * friction;
                player.rb.AddForce(frictionForce, ForceMode.Acceleration);
            }
        }
    }

    void Move(float maxSpeedTime)
    {
        player.FlatVel = new Vector3(player.rb.velocity.x, 0f, player.rb.velocity.z);

        Vector3 moveDirection = player.GetMoveDirection();

        player.rb.AddForce(Vector3.down);

        float targetSpeed = moveDirection.magnitude * maxSpeed;
        Vector3 targetVelocity = moveDirection * targetSpeed;
        Vector3 velocityChange = (targetVelocity - player.FlatVel) * maxSpeedTime;

        player.rb.AddForce(velocityChange * accelAmount, ForceMode.Acceleration);
    }
}

