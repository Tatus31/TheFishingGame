using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class SwimmingState : MovementBaseState
{
    PlayerMovement player;

    float maxSpeed;
    float accelAmount;
    float buoyancy = 1f; 
    float waterDrag = 0.5f; 
    float submergenceRange = 10f;
    float submergenceOffset = 0.5f;
    float submergence;
    float gravityMultiplier = 0.02f;

    Vector3 upAxis = Vector3.up;

    bool isSinking;

    public SwimmingState(PlayerMovement player, float maxSpeed, float accelAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
    }

    public override void EnterState(PlayerMovement player)
    {
        ClimbLadderToShip.OnClimb += ClimbLadderToShip_OnClimb;

        this.player = player;

        PlayerSetup();
    }

    private void ClimbLadderToShip_OnClimb(bool b)
    {
        isSinking = b;
    }

    public override void ExitState()
    {
        if (player != null)
        {
            player.rb.useGravity = true;
        }
    }

    public override void UpdateState()
    {
        EvaluateSubmergence();
    }

    public override void FixedUpdateState()
    {
        Move(player, maxSpeed, maxSpeed, accelAmount);
    }

    public override void Move(PlayerMovement player, float maxSpeedTime, float maxSpeed, float accelAmount)
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraRight.y = 0;
        cameraRight.Normalize();

        float horizontalInput = player.inputManager.GetPlayerMovement().x;
        float verticalInput = player.inputManager.GetPlayerMovement().y;

        Vector3 swimDirection = (cameraForward * verticalInput) + (cameraRight * horizontalInput);
        swimDirection.Normalize();
        Vector3 targetVelocity = swimDirection * maxSpeed;

        if(!isSinking)
            player.rb.velocity *= 1f - (waterDrag * Time.deltaTime);

        Vector3 velocityChange = (targetVelocity - player.rb.velocity);

        player.rb.AddForce(velocityChange * accelAmount, ForceMode.Acceleration);
        player.rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

        if (submergence > 0.5f && swimDirection.magnitude < 0.1f)
        {
            float buoyancyForce = buoyancy * submergence;
            player.rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
        }
        player.FlatVel = new Vector3(player.rb.velocity.x, 0f, player.rb.velocity.z);
    }

    void EvaluateSubmergence()
    {
        if (Physics.Raycast(
            player.rb.position + upAxis * submergenceOffset,
            -upAxis,
            out RaycastHit hit,
            submergenceRange + 1f,
            player.gameObject.layer,
            QueryTriggerInteraction.Collide
        ))
        {
            submergence = 1f - hit.distance / submergenceRange;
        }
        else
        {
            submergence = 1f;
        }
    }

    void PlayerSetup()
    {
        player.rb.useGravity = false;

        //Vector3 horizontalVelocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);
        //player.rb.velocity = horizontalVelocity;
    }

    public override void PlayAnimation(PlayerMovement player)
    {
        //if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.Empty))
        //    player.GetAnimationController().PlayAnimation(player.GetFreeHandAnimator(), AnimationController.ON_RUN, false);
        //else if (InteractionManager.Instance.IsToolEquipped(InteractionManager.EquipedTool.FishingRod))
        //    player.GetAnimationController().PlayAnimation(player.GetFishingAnimator(), AnimationController.ON_RUN, false);
    }

    public override void DrawGizmos(PlayerMovement player)
    {
        Gizmos.DrawLine(player.transform.position, Vector3.down * submergenceRange);
        Gizmos.color = Color.yellow;
    }
}