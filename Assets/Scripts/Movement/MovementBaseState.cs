using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class MovementBaseState
{
    public virtual void EnterState(PlayerMovement player)
    {
        PlayAnimation(player);

        Debug.Log($"Entered state {player.CurrentState}");
    }
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public virtual void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        if (player.GetMoveDirection().magnitude < 0.01f)
        {
            Vector3 horizontalVelocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);
            if (horizontalVelocity.magnitude > 0)
            {
                //float friction = Mathf.Min(horizontalVelocity.magnitude, frictionAmount);
                Vector3 frictionForce = -horizontalVelocity.normalized * frictionAmount;
                player.rb.AddForce(frictionForce, ForceMode.Acceleration);
            }
        }
    }
    public virtual void Move(PlayerMovement player, float maxSpeedTime, float maxSpeed, float accelAmount)
    {
        player.FlatVel = new Vector3(player.rb.velocity.x, 0f, player.rb.velocity.z);

        Vector3 moveDirection = player.GetMoveDirection();

        player.rb.AddForce(Vector3.down);

        float targetSpeed = moveDirection.magnitude * maxSpeed;
        Vector3 targetVelocity = moveDirection * targetSpeed;
        Vector3 velocityChange = (targetVelocity - player.FlatVel) * maxSpeedTime;

        player.rb.AddForce(velocityChange * accelAmount, ForceMode.Acceleration);
    }

    public abstract void PlayAnimation(PlayerMovement player);

    public virtual void DrawGizmos(PlayerMovement player) { }
}
