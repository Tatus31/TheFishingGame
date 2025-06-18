using UnityEngine;

public abstract class MovementBaseState
{
    protected Vector3 contactNormal = Vector3.up;
    protected int groundContactCount = 0;
    protected bool OnGround => groundContactCount > 0;

    protected float maxGroundAngle = 90f;
    protected float minGroundDotProduct;

    Vector3 debugMoveDirection;
    Vector3 debugContactNormal;
    Vector3 debugForceDirection;
    float debugForceStrength;

    Collider playerCollider;

    public MovementBaseState()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minGroundDotProduct = Mathf.Abs(minGroundDotProduct);
    }

    public virtual void EnterState(PlayerMovement player)
    {
        PlayAnimation(player);

        playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null)
        {
            Debug.LogWarning("player has no collider");
        }

        contactNormal = Vector3.up;
        groundContactCount = 0;
    }
    public abstract void ExitState();
    public abstract void UpdateState();

    public virtual void UpdateState(PlayerMovement player)
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;

        CheckGroundContacts(player);

        if (groundContactCount > 0)
        {
            contactNormal.Normalize();
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    public abstract void FixedUpdateState();
    public virtual void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        if (player.GetMoveDirection().magnitude < 0.01f)
        {
            Vector3 horizontalVelocity = new Vector3(player.rb.velocity.x, 0, player.rb.velocity.z);

            //AudioManager.MuteSound(AudioManager.WalkSound);

            if (horizontalVelocity.magnitude > 0)
            {
                Vector3 frictionForce = -horizontalVelocity.normalized * frictionAmount;
                player.rb.AddForce(frictionForce, ForceMode.Acceleration);
            }
        }
    }
    public virtual void Move(PlayerMovement player, float maxSpeedTime, float maxSpeed, float accelAmount)
    {
        player.FlatVel = new Vector3(player.rb.velocity.x, 0f, player.rb.velocity.z);
        Vector3 moveDirection = player.GetMoveDirection();

        //AudioManager.PlaySound(AudioManager.WalkSound);

        debugMoveDirection = moveDirection;
        debugContactNormal = contactNormal;

        player.rb.AddForce(Vector3.down);

        float targetSpeed = moveDirection.magnitude * maxSpeed;

        if (OnGround && contactNormal.y < 0.99f)
        {
            moveDirection = ProjectOnContactPlane(moveDirection).normalized;
        }

        Vector3 targetVelocity = moveDirection * targetSpeed;
        Vector3 velocityChange = (targetVelocity - player.FlatVel) * maxSpeedTime;

        debugForceDirection = velocityChange.normalized;
        debugForceStrength = velocityChange.magnitude * accelAmount;

        player.rb.AddForce(velocityChange * accelAmount, ForceMode.Acceleration);
    }

    public abstract void PlayAnimation(PlayerMovement player);

    public virtual void DrawGizmos(PlayerMovement player)
    {
        Vector3 playerPos = player.transform.position;
        float scale = 2.0f;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerPos, playerPos + debugContactNormal * scale);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerPos, playerPos + debugMoveDirection * scale);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPos, playerPos + debugForceDirection * (scale * Mathf.Min(debugForceStrength / 10f, 3f)));
        Gizmos.DrawSphere(playerPos + debugForceDirection * (scale * Mathf.Min(debugForceStrength / 10f, 3f)), 0.1f);
    }

    protected Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    public virtual void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount++;
                contactNormal += normal;
            }
        }
    }

    protected virtual void CheckGroundContacts(PlayerMovement player)
    {
        if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            if (hit.normal.y >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = hit.normal;
            }
        }
    }
}
