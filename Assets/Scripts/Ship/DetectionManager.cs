using System;
using UnityEngine;
using static ShipMovement;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance { get; private set; }
    [SerializeField] Transform shipTransform;
    [SerializeField] Transform monsterHead;
    [SerializeField] float initialDetectionTimer = 200f;
    [SerializeField] float detectionTimerDecreaseRate = 1f;
    [SerializeField] float baseInvestigationPointUpdateInterval = 2f;
    [SerializeField] float minDistanceToShip = 10f;
    [SerializeField] float investigationCooldown = 10f;

    float currentDetectionTimer;
    float investigationPointUpdateTimer;
    float currentInvestigationPointUpdateInterval;
    float currentCooldownTimer = 0f;

    Vector3 investigationTargetPoint;

    bool isInvestigating = false;
    bool isDecoyActive = false;
    bool isLightsActive = false;
    bool isLightsFlickerActive = false;
    bool isCollisionActive = false;
    bool isisHuntingPlayer = false;

    SpeedLevel speedLevel;

    public bool IsInvestigating { get { return isInvestigating; } set { isInvestigating = value; } }

    [Serializable]
    public class DetectionValues
    {
        public float decoyDetection = 6f;
        public float lightsDetection = 1f;
        public float lightsFlickerDetection = 2f;
        public float collisionDetection = 1f;
        public float reverseSpeedDetection = 0.5f;
        public float neutralSpeedDetection = 0f;
        public float forward1SpeedDetection = 1f;
        public float forward2SpeedDetection = 2f;
        public float forward3SpeedDetection = 3f;
    }

    [SerializeField] DetectionValues detectionValues;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        OnDetectionChange += ShipMovement_OnDetectionChange;
        LightsManager.OnLightsToggled += LightsManager_OnLightsToggled;
        LightsManager.OnLightsFlicker += LightsManager_OnLightsFlicker;
        Decoy.OnDecoyActivated += Decoy_OnDecoyActivated;
    }

    private void Decoy_OnDecoyActivated(object sender, Transform e)
    {
        //MonsterStateMachine.Instance.InvestigatingState.SetInvestigationRadius(detectionValues.decoyDetection);
        //MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.InvestigatingState);
        //Debug.Log("decoy deployed");
        //isSearching = true;
    }

    private void LightsManager_OnLightsFlicker(object sender, bool e)
    {
        isLightsFlickerActive = e;
    }

    private void LightsManager_OnLightsToggled(object sender, bool e)
    {
        isLightsActive = e;
    }

    private void OnValidate()
    {
        //Debug.Log($"detection values: \n{detectionValues.collisionDetection} \n {detectionValues.reverseSpeedDetection}\n {detectionValues.neutralSpeedDetection}" +
        //    $"\n {detectionValues.forward1SpeedDetection}\n {detectionValues.forward2SpeedDetection}\n {detectionValues.forward3SpeedDetection}");
    }

    private void ShipMovement_OnDetectionChange(object sender, ShipMovement.SpeedLevel e)
    {
        speedLevel = e;

        //Debug.Log($"current detection: {currentDetectionMultiplier} for: {e}");
    }

    private void Update()
    {
        if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
            return; 
        }

        if (isisHuntingPlayer)
        {
            return;
        }

        float distance = Vector3.Distance(shipTransform.position, monsterHead.position);
        if (distance <= minDistanceToShip && !isInvestigating)
        {
            if (MonsterLargeStateMachine.Instance != null)
            {
                MonsterLargeStateMachine.Instance.SwitchState(MonsterLargeStateMachine.Instance.InvestigatingState);
                isInvestigating = true;
            }
        }
    }

    public void StartInvestigation(Transform shipTransform)
    {
        currentDetectionTimer = initialDetectionTimer;
        investigationTargetPoint = shipTransform.position;
        UpdateInvestigationInterval();
        investigationPointUpdateTimer = currentInvestigationPointUpdateInterval;
    }

    public void UpdateInvestigationPoint(Transform shipTransform)
    {
        investigationPointUpdateTimer -= Time.deltaTime;
        if (investigationPointUpdateTimer <= 0)
        {
            investigationTargetPoint = shipTransform.position;
            UpdateInvestigationInterval();
            investigationPointUpdateTimer = currentInvestigationPointUpdateInterval;
        }
    }

    void UpdateInvestigationInterval()
    {
        float interval = baseInvestigationPointUpdateInterval;
        float totalReductionPercent = 0f;

        if (isDecoyActive)
            totalReductionPercent += detectionValues.decoyDetection;

        if (isLightsActive)
            totalReductionPercent += detectionValues.lightsDetection;

        if (isLightsFlickerActive)
            totalReductionPercent += detectionValues.lightsFlickerDetection;

        if (isCollisionActive)
            totalReductionPercent += detectionValues.collisionDetection;

        switch (speedLevel)
        {
            case SpeedLevel.reverse:
                totalReductionPercent += detectionValues.reverseSpeedDetection;
                break;
            case SpeedLevel.neutral:
                totalReductionPercent += detectionValues.neutralSpeedDetection;
                break;
            case SpeedLevel.forward1:
                totalReductionPercent += detectionValues.forward1SpeedDetection;
                break;
            case SpeedLevel.forward2:
                totalReductionPercent += detectionValues.forward2SpeedDetection;
                break;
            case SpeedLevel.forward3:
                totalReductionPercent += detectionValues.forward3SpeedDetection;
                break;
        }

        currentInvestigationPointUpdateInterval = interval * (1f - (totalReductionPercent / 10f));
        currentInvestigationPointUpdateInterval = Mathf.Max(0.1f, currentInvestigationPointUpdateInterval);
    }

    public void DecreaseDetectionTimer()
    {
        currentDetectionTimer -= detectionTimerDecreaseRate * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (shipTransform == null || monsterHead == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(shipTransform.position, monsterHead.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(investigationTargetPoint, 1f);

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        string timerLabel = $"Detection Timer: {currentDetectionTimer:F2}" + $"\nInvestigation point interval {currentInvestigationPointUpdateInterval:F2}";
        Vector3 labelPosition = (monsterHead.position);
        UnityEditor.Handles.Label(labelPosition + Vector3.down, timerLabel);
#endif
    }

    public void StartCooldown()
    {
        currentCooldownTimer = investigationCooldown;
    }

    public bool ShouldContinueInvestigation() => currentDetectionTimer > 0;
    public Vector3 GetInvestigationPoint() => investigationTargetPoint;
    public bool IsInCooldown => currentCooldownTimer > 0f;
    public void SetDecoyActive(bool active) => isDecoyActive = active;
    public void SetLightsActive(bool active) => isLightsActive = active;
    public void SetLightsFlickerActive(bool active) => isLightsFlickerActive = active;
    public void SetCollisionActive(bool active) => isCollisionActive = active;
    public void SetSpeedState(SpeedLevel state) => speedLevel = state;

}