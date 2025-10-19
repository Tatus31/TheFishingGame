using System;
using System.Collections;
using UnityEngine;

public class LakeMonsterAttackingState : BaseLakeMonsterState
{
    Transform shipTransform;
    Transform playerTransform;
    Transform monsterTransform;
    ShipMovement shipMovement;
    Rigidbody rb;

    float swimAttackSpeed;
    float monsterEscapeTime;
    float turnSmoothTime;
    float predictionValue;
    float attackDuration;
    float maxAttackDuration;
    float windUpTimer;
    float windUpDuration = 1.5f;
    float windUpRotationSpeed = 2f;
    float stopPredictionAttackRange = 60f;
    float maxPitch = 1.3f;

    int numberOfAttacks;
    int maxNumberOfAttacks = 3;

    bool isMonsterRetreating;
    bool isPlayerSwimming;
    bool shipSank;
    bool isMonsterPursuing;
    bool isWindingUp;

    Vector3 targetDirection;
    Vector3 directionToShip;
    Vector3 currentMoveDirection;

    public LakeMonsterAttackingState(
        Transform shipTransform, Transform monsterTransform, Transform playerTransform, ShipMovement shipMovement, float swimAttackSpeed, Rigidbody rb, float monsterEscapeTime, float maxAttackDuration,
        float turnSmoothTime, int maxNumberOfAttacks, float predictionValue, float windUpDuration, float windUpRotationSpeed, float stopPredictionAttackRange)
    {
        this.shipTransform = shipTransform;
        this.monsterTransform = monsterTransform;
        this.playerTransform = playerTransform;
        this.shipMovement = shipMovement;
        this.swimAttackSpeed = swimAttackSpeed;
        this.rb = rb;
        this.monsterEscapeTime = monsterEscapeTime;
        this.maxAttackDuration = maxAttackDuration;
        this.turnSmoothTime = turnSmoothTime;
        this.maxNumberOfAttacks = maxNumberOfAttacks;
        this.predictionValue = predictionValue;
        this.windUpDuration = windUpDuration;
        this.windUpRotationSpeed = windUpRotationSpeed;
        this.stopPredictionAttackRange = stopPredictionAttackRange;

    }

    public override void EnterState(LakeMonsterStateMachine monsterState)
    {
        Debug.Log($"Entering Attacking State {monsterTransform.name}");
        AudioManager.ChangeAudioPitch(AudioManager.HeartBeatSound, maxPitch);
        InitializeAttackState();

        SubscribeEvents();
        SetTargetDirection();
    }

    public override void ExitState()
    {
        UnsubscribeEvents();
        CameraOverlayManager.Instance.EndEvent();
        isMonsterPursuing = false;
    }

    public override void UpdateState(LakeMonsterStateMachine monsterState)
    {
        HandleMonsterRotation(monsterState);
        if (isWindingUp)
        {
            HandleWindUpRotation();
            return;
        }

        HandleAttackTimer();
    }

    public override void FixedUpdateState(LakeMonsterStateMachine monsterState)
    {
        if (isWindingUp) return;

        MoveMonster();
        HandleRetreat(monsterState);
    }

    void InitializeAttackState()
    {
        CameraOverlayManager.Instance.TriggerEventWithDelay();
        AudioManager.PlaySound(AudioManager.HeartBeatSound);

        if(shipMovement.IsControllingShip)
            CameraTransitionManager.Instance.EnableAttackCamera();

        isMonsterPursuing = true;
        isWindingUp = true;
        windUpTimer = 0f;
        attackDuration = 0f;
        isMonsterRetreating = false;
    }

    void SubscribeEvents()
    {
        ShipDamage.Instance.OnDamageTaken += OnShipDamageTaken;
        PlayerMovement.Instance.OnPlayerSwimmingChange += OnPlayerSwimmingChange;
        SinkShip.OnShipSank += OnShipSank;
        DetectionManager.OnInvestigationEnd += OnInvestigationEnd;
    }

    void UnsubscribeEvents()
    {
        ShipDamage.Instance.OnDamageTaken -= OnShipDamageTaken;
        PlayerMovement.Instance.OnPlayerSwimmingChange -= OnPlayerSwimmingChange;
        SinkShip.OnShipSank -= OnShipSank;
        DetectionManager.OnInvestigationEnd -= OnInvestigationEnd;
    }

    void OnInvestigationEnd() => isMonsterPursuing = false;

    void OnShipSank(bool sank) => shipSank = sank;

    void OnPlayerSwimmingChange(object sender, bool swimming)
    {
        isPlayerSwimming = swimming;
        SetTargetDirection();
    }

    void OnShipDamageTaken(object sender, int e)
    {
        numberOfAttacks++;
        attackDuration = 0f;
        StartRetreatAndWindUp();
    }

    void StartRetreatAndWindUp()
    {
        isMonsterRetreating = true;
        isWindingUp = true;
        windUpTimer = 0f;
        SetTargetDirection();
    }

    void SetTargetDirection()
    {
        float distanceToShip = Vector3.Distance(monsterTransform.position, shipTransform.position);

        Vector3 predictedPosition;

        if (isPlayerSwimming)
        {
            predictedPosition = playerTransform.position;
        }
        else if (distanceToShip <= stopPredictionAttackRange)
        {
            predictedPosition = AttackShipPosition();
        }
        else
        {
            predictedPosition = PredictShipPosition();
        }

        directionToShip = (predictedPosition - monsterTransform.position).normalized;
        targetDirection = isMonsterRetreating ? -directionToShip : directionToShip;
    }

    Vector3 PredictShipPosition()
    {
        if (shipTransform == null)
            return Vector3.zero;

        Vector3 predictedPosition = shipTransform.position;

        if (shipMovement != null)
        {
            Vector3 shipVelocity = shipMovement.ShipFlatVel;

            if (shipVelocity.magnitude > 0.1f)
            {
                predictedPosition += shipVelocity.normalized * shipVelocity.magnitude * predictionValue;
            }
        }

        return predictedPosition;
    }

    Vector3 AttackShipPosition()
    {
        if (shipTransform == null)
            return Vector3.zero;

        Vector3 attackPosition = shipTransform.position;

        //if (shipMovement != null)
        //{
        //    Vector3 shipVelocity = shipMovement.ShipFlatVel;
        //    attackPosition += shipVelocity * 0.1f;
        //}

        return attackPosition;
    }

    void HandleMonsterRotation(LakeMonsterStateMachine monsterState)
    {
        if (rb.velocity.magnitude > 0.1f)
            monsterState.LookAt(rb.velocity.normalized);
    }

    void HandleWindUpRotation()
    {
        windUpTimer += Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(directionToShip);monsterTransform.rotation = Quaternion.Slerp(monsterTransform.rotation, targetRotation, Time.deltaTime * windUpRotationSpeed);

        if (windUpTimer >= windUpDuration)
        {
            isWindingUp = false;
            windUpTimer = 0f;
        }
    }

    void HandleAttackTimer()
    {
        if (isMonsterRetreating) return;

        attackDuration += Time.deltaTime;
        if (attackDuration > maxAttackDuration)
        {
            numberOfAttacks++;

            Debug.Log($"Monster missed attack {numberOfAttacks}");

            isMonsterRetreating = true;
            attackDuration = 0f;

            SetTargetDirection();
        }
    }

    void MoveMonster()
    {
        currentMoveDirection = Vector3.Lerp(currentMoveDirection, targetDirection, Time.fixedDeltaTime / turnSmoothTime);
        rb.AddForce(currentMoveDirection * swimAttackSpeed, ForceMode.Acceleration);
    }

    void HandleRetreat(LakeMonsterStateMachine monsterState)
    {
        if (isMonsterRetreating)
            monsterState.StartCoroutine(SwimAwayFromShip(monsterState));
    }

    IEnumerator SwimAwayFromShip(LakeMonsterStateMachine monsterState)
    {
        yield return new WaitForSeconds(monsterEscapeTime);

        if (numberOfAttacks >= maxNumberOfAttacks || shipSank || !isMonsterPursuing)
        {
            numberOfAttacks = 0;

            monsterState.SwitchState(monsterState.IdleState);
        }

        isMonsterRetreating = false;
        attackDuration = 0f;
        SetTargetDirection();
    }

    public override void DrawGizmos(LakeMonsterStateMachine monsterState)
    {
        if (monsterTransform == null)
            return;

        Gizmos.color = isMonsterRetreating ? Color.blue : Color.red;
        Gizmos.DrawRay(monsterTransform.position, currentMoveDirection * 5f);

        Transform currentTarget = isPlayerSwimming ? playerTransform : shipTransform;

        if (currentTarget == null)
            return;

        float sphereSize = isMonsterRetreating ? 2f : 1f;
        Gizmos.DrawWireSphere(currentTarget.position, sphereSize);

        Vector3 targetPosition;

        if (isPlayerSwimming)
        {
            targetPosition = playerTransform.position;
        }
        else
        {
            float distanceToShip = Vector3.Distance(monsterTransform.position, shipTransform.position);

            if (distanceToShip <= stopPredictionAttackRange)
            {
                targetPosition = AttackShipPosition();
                Gizmos.color = Color.magenta; 
            }
            else
            {
                targetPosition = PredictShipPosition();
                Gizmos.color = Color.yellow;
            }
        }

        Gizmos.DrawWireSphere(targetPosition, 1.5f);
        Gizmos.DrawLine(monsterTransform.position, targetPosition);
    }

}
