using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static ShipMovement;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance { get; private set; }

    public static event Action OnInvestigationEnd;

    [SerializeField] Transform shipTransform;
    [SerializeField] Transform[] monsterHeads;
    [SerializeField] float initialDetectionTimer = 200f;
    [SerializeField] float detectionTimerDecreaseRate = 1f;
    [SerializeField] float investigationPointUpdateTime = 2f;
    [SerializeField] float minDistanceToShip = 10f;
    [SerializeField] float maxDistanceToShip = 30f;
    [SerializeField] float investigationCooldown = 10f;

    Dictionary<Transform, MonsterDetectionState> monsterStates = new Dictionary<Transform, MonsterDetectionState>();
    Dictionary<Transform, MonsterType> monsterTypes = new Dictionary<Transform, MonsterType>();

    float investigationPointUpdateTimer;
    float currentInvestigationPointUpdateInterval;

    Vector3 investigationTargetPoint;

    bool isDecoyActive = false;
    bool isLightsActive = false;
    bool isLightsFlickerActive = false;
    bool isCollisionActive = false;
    bool isHuntingPlayer = false;

    SpeedLevel speedLevel;

    public enum MonsterType
    {
        Large,
        Medium,
        Small,
        NotRecognized
    }

    [Serializable]
    private class MonsterDetectionState
    {
        public float currentDetectionTimer;
        public float currentCooldownTimer = 0f;
        public bool isInvestigating = false;
    }

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
        InitializeMonsterStates();

        OnDetectionChange += ShipMovement_OnDetectionChange;
        LightsManager.OnLightsToggled += LightsManager_OnLightsToggled;
        LightsManager.OnLightsFlicker += LightsManager_OnLightsFlicker;
        Decoy.OnDecoyActivated += Decoy_OnDecoyActivated;
    }

    private void InitializeMonsterStates()
    {
        monsterStates.Clear();
        monsterTypes.Clear();

        foreach (Transform head in monsterHeads)
        {
            if (head != null)
            {
                monsterStates.Add(head, new MonsterDetectionState
                {
                    currentDetectionTimer = 0f,
                    currentCooldownTimer = 0f,
                    isInvestigating = false
                });

                if (head.GetComponentInParent<MonsterLargeStateMachine>() != null)
                {
                    monsterTypes.Add(head, MonsterType.Large);
                }
                else if (head.GetComponentInParent<MediumMonsterStateMachine>() != null && head.GetComponentInParent<MediumMonsterStateMachine>().isSmallMonster)
                {
                    monsterTypes.Add(head, MonsterType.Small);
                }
                else if (head.GetComponentInParent<MediumMonsterStateMachine>() != null)
                {
                    monsterTypes.Add(head, MonsterType.Medium);
                }
                else
                {
                    Debug.LogWarning($"Monster type not recognized for {head.name}. Please assign a valid type.");
                }
            }
        }
    }

    private void Decoy_OnDecoyActivated(object sender, Transform e)
    {
        isDecoyActive = true;

        foreach (var monsterHead in monsterStates.Keys)
        {
            if (IsInCooldown(monsterHead)) continue;

            StartDecoyInvestigation(monsterHead, e);
        }
    }

    public void StartDecoyInvestigation(Transform monster, Transform decoyTransform)
    {
        if (!monsterStates.ContainsKey(monster) || !monsterTypes.ContainsKey(monster)) return;

        var state = monsterStates[monster];
        state.isInvestigating = true;
        state.currentDetectionTimer = initialDetectionTimer;

        investigationTargetPoint = decoyTransform.position;

        switch (monsterTypes[monster])
        {
            case MonsterType.Large:
                var largeStateMachine = monster.GetComponentInParent<MonsterLargeStateMachine>();
                if (largeStateMachine != null)
                {
                    largeStateMachine.SwitchState(largeStateMachine.InvestigatingState);
                }
                break;

            case MonsterType.Medium:
                var mediumStateMachine = monster.GetComponentInParent<MediumMonsterStateMachine>();
                if (mediumStateMachine != null && mediumStateMachine.InvestigatingState != null)
                {
                    mediumStateMachine.SwitchState(mediumStateMachine.InvestigatingState);
                }
                break;
            case MonsterType.Small:
                var smallStateMachine = monster.GetComponentInParent<MediumMonsterStateMachine>();
                if (smallStateMachine != null && smallStateMachine.InvestigatingState != null && smallStateMachine.isSmallMonster)
                {
                    smallStateMachine.SwitchState(smallStateMachine.InvestigatingState);
                }
                break;
        }
    }

    private void LightsManager_OnLightsFlicker(object sender, bool e)
    {
        isLightsFlickerActive = e;
    }

    private void LightsManager_OnLightsToggled(object sender, bool e)
    {
        isLightsActive = e;
    }

    private void ShipMovement_OnDetectionChange(object sender, ShipMovement.SpeedLevel e)
    {
        speedLevel = e;
    }

    private void Update()
    {
        if (isHuntingPlayer)
        {
            return;
        }

        foreach (Transform monsterHead in monsterHeads)
        {
            if (monsterHead == null || !monsterStates.ContainsKey(monsterHead) || !monsterTypes.ContainsKey(monsterHead))
                continue;

            var state = monsterStates[monsterHead];

            if (state.currentCooldownTimer > 0f)
            {
                state.currentCooldownTimer -= Time.deltaTime;
                continue;
            }

            float distance = Vector3.Distance(shipTransform.position, monsterHead.position);

            if (state.isInvestigating)
            {
                if (distance > maxDistanceToShip)
                {
                    EndInvestigation(monsterHead);
                    continue; 
                }

                UpdateInvestigationPoint(monsterHead, shipTransform);
                DecreaseDetectionTimer(monsterHead);

                if (!ShouldContinueInvestigation(monsterHead))
                {
                    EndInvestigation(monsterHead);
                }
            }
            else
            {
                if (distance <= minDistanceToShip)
                {
                    switch (monsterTypes[monsterHead])
                    {
                        case MonsterType.Large:
                            var largeStateMachine = monsterHead.GetComponentInParent<MonsterLargeStateMachine>();
                            if (largeStateMachine != null)
                            {
                                largeStateMachine.SwitchState(largeStateMachine.InvestigatingState);
                                state.isInvestigating = true;
                                StartInvestigation(monsterHead, shipTransform);
                            }
                            break;

                        case MonsterType.Medium:
                            var mediumStateMachine = monsterHead.GetComponentInParent<MediumMonsterStateMachine>();
                            if (mediumStateMachine != null && mediumStateMachine.InvestigatingState != null)
                            {
                                mediumStateMachine.SwitchState(mediumStateMachine.InvestigatingState);
                                state.isInvestigating = true;
                                StartInvestigation(monsterHead, shipTransform);
                            }
                            break;
                        case MonsterType.Small:
                            var smallStateMachine = monsterHead.GetComponentInParent<MediumMonsterStateMachine>();
                            if (smallStateMachine != null && smallStateMachine.InvestigatingState != null && smallStateMachine.isSmallMonster)
                            {
                                smallStateMachine.SwitchState(smallStateMachine.InvestigatingState);
                            }
                            break;
                    }
                }
            }
        }
    }


    public void StartInvestigation(Transform monster, Transform target)
    {
        if (!monsterStates.ContainsKey(monster)) return;

        var state = monsterStates[monster];
        state.currentDetectionTimer = initialDetectionTimer;
        investigationTargetPoint = target.position;
        UpdateInvestigationInterval();
        investigationPointUpdateTimer = currentInvestigationPointUpdateInterval;
    }

    public void UpdateInvestigationPoint(Transform monster, Transform target)
    {
        if (!monsterStates.ContainsKey(monster)) return;

        investigationPointUpdateTimer -= Time.deltaTime;
        if (investigationPointUpdateTimer <= 0)
        {
            investigationTargetPoint = target.position;
            UpdateInvestigationInterval();
            investigationPointUpdateTimer = currentInvestigationPointUpdateInterval;
        }
    }

    void EndInvestigation(Transform monster)
    {
        if (!monsterStates.ContainsKey(monster) || !monsterTypes.ContainsKey(monster)) return;

        var state = monsterStates[monster];
        state.isInvestigating = false;
        StartCooldown(monster);

        switch (monsterTypes[monster])
        {
            case MonsterType.Large:
                var largeStateMachine = monster.GetComponentInParent<MonsterLargeStateMachine>();
                if (largeStateMachine != null)
                {
                    largeStateMachine.SwitchState(largeStateMachine.IdleState);
                    OnInvestigationEnd?.Invoke();
                }
                break;

            case MonsterType.Medium:
                var mediumStateMachine = monster.GetComponentInParent<MediumMonsterStateMachine>();
                if (mediumStateMachine != null)
                {
                    mediumStateMachine.SwitchState(mediumStateMachine.IdleState);
                    OnInvestigationEnd?.Invoke();
                }
                break;

            case MonsterType.Small:
                var smallStateMachine = monster.GetComponentInParent<MediumMonsterStateMachine>();
                if (smallStateMachine != null && smallStateMachine.isSmallMonster)
                {
                    smallStateMachine.SwitchState(smallStateMachine.IdleState);
                    OnInvestigationEnd?.Invoke();
                }
                break;
        }
    }

    void UpdateInvestigationInterval()
    {
        float interval = investigationPointUpdateTime;
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

    public void DecreaseDetectionTimer(Transform monster)
    {
        if (!monsterStates.ContainsKey(monster)) return;

        var state = monsterStates[monster];
        state.currentDetectionTimer -= detectionTimerDecreaseRate * Time.deltaTime;
    }

    public void StartCooldown(Transform monster)
    {
        if (!monsterStates.ContainsKey(monster)) return;

        var state = monsterStates[monster];
        state.currentCooldownTimer = investigationCooldown;
    }

    public bool ShouldContinueInvestigation(Transform monster)
    {
        if (!monsterStates.ContainsKey(monster)) return false;
        return monsterStates[monster].currentDetectionTimer > 0;
    }

    public Vector3 GetInvestigationPoint()
    {
        return investigationTargetPoint;
    }

    public bool IsInCooldown(Transform monster)
    {
        if (!monsterStates.ContainsKey(monster)) return false;
        return monsterStates[monster].currentCooldownTimer > 0f;
    }

    public MonsterType GetMonsterType(Transform monster)
    {
        if (monsterTypes.ContainsKey(monster))
        {
            return monsterTypes[monster];
        }

        return MonsterType.NotRecognized;
    }

    private void OnDrawGizmos()
    {
        if (shipTransform == null || monsterHeads.Length == 0) return;

        foreach (Transform monsterHead in monsterHeads)
        {
            if (monsterHead == null) continue;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(shipTransform.position, monsterHead.position);

            if (monsterStates.ContainsKey(monsterHead) && monsterStates[monsterHead].isInvestigating)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(investigationTargetPoint, 1f);

#if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                string monsterType = monsterTypes.ContainsKey(monsterHead) ? monsterTypes[monsterHead].ToString() : "no name";
                float distance = Vector3.Distance(shipTransform.position, monsterHead.position);
                string timerLabel = $"{monsterType} Monster\n" +
                                    $"Detection Timer: {monsterStates[monsterHead].currentDetectionTimer:F2}\n" +
                                    $"Investigation Interval: {currentInvestigationPointUpdateInterval:F2}\n" +
                                    $"Distance to Ship: {distance:F2}m\n" +
                                    $"Max distance from ship {maxDistanceToShip:F2}m";
                Vector3 labelPosition = monsterHead.position;
                UnityEditor.Handles.Label(labelPosition + Vector3.down, timerLabel);
#endif
            }
        }
    }

}