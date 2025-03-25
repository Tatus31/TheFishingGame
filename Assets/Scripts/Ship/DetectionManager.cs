using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipMovement;

public class DetectionManager : MonoBehaviour
{
    public static DetectionManager Instance;

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

    [Header("Detection Setup")]
    [Space(10)]
    [SerializeField] float detectionMultiplierValue;
    [SerializeField]
    [Tooltip("time to interpolate between current detection and the target one")] float lerpTime = 1f;
    [SerializeField] float timer = 5f;
    [SerializeField] float staticValueTimer = 10f;
    [Space(10)]
    [SerializeField] DetectionValues detectionValues;

    float time;
    float currentDetectionMultiplier;
    SpeedLevel speedLevel;

    float additionalDetection = 0f;
    float collisionDetectionTimer = 0f;

    float monsterMinimalDetectionValue = 0.4f;
    float monsterMinimalDistanceFromShip = 50f;

    bool isSearching = false;
    bool setUsingEditor;

    bool areLightsOn;
    bool areLightsFlickering;

    public float CurrentDetectionMultiplier { get { return currentDetectionMultiplier; } set { currentDetectionMultiplier = value; } }
    public DetectionValues GetDetectionValues => detectionValues;
    public bool SetUsingEditor { get { return setUsingEditor; } set { setUsingEditor = value; } }   

    private void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }

        Instance = this;
    }

    private void Start()
    {
        currentDetectionMultiplier = 0;
        speedLevel = SpeedLevel.neutral;

        OnDetectionChange += ShipMovement_OnDetectionChange;
        ShipDamage.Instance.OnDetectionChange += ShipDamage_OnDetectionChange;
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
        areLightsFlickering = e;
    }

    private void LightsManager_OnLightsToggled(object sender, bool e)
    {
        areLightsOn = e;
    }

    private void OnValidate()
    {
        //Debug.Log($"detection values: \n{detectionValues.collisionDetection} \n {detectionValues.reverseSpeedDetection}\n {detectionValues.neutralSpeedDetection}" +
        //    $"\n {detectionValues.forward1SpeedDetection}\n {detectionValues.forward2SpeedDetection}\n {detectionValues.forward3SpeedDetection}");
    }

    private void ShipDamage_OnDetectionChange(object sender, float detectionValue)
    {
        additionalDetection = detectionValue;
        currentDetectionMultiplier += additionalDetection;

        collisionDetectionTimer = staticValueTimer;

#if UNITY_EDITOR
        Debug.Log($"added {detectionValue} to detection new value: {currentDetectionMultiplier}");
#endif
    }

    private void ShipMovement_OnDetectionChange(object sender, ShipMovement.SpeedLevel e)
    {
        speedLevel = e;

        //Debug.Log($"current detection: {currentDetectionMultiplier} for: {e}");
    }

    private void Update()
    {
        if (collisionDetectionTimer > 0)
        {
            collisionDetectionTimer -= Time.deltaTime;
            if (collisionDetectionTimer <= 0)
            {
                currentDetectionMultiplier -= additionalDetection;
                additionalDetection = 0f;
#if UNITY_EDITOR
                Debug.Log("collision detection over");
#endif
            }
        }

        float targetValue = 0f;
        float lightDetectionBonus = 0f;

        if (areLightsOn)
        {
            lightDetectionBonus = detectionValues.lightsDetection;
        }
        else if (areLightsFlickering)
        {
            lightDetectionBonus = detectionValues.lightsFlickerDetection;
        }

        switch (speedLevel)
        {
            case SpeedLevel.reverse:
                targetValue = detectionValues.reverseSpeedDetection;
                break;
            case SpeedLevel.neutral:
                targetValue = detectionValues.neutralSpeedDetection;
                break;
            case SpeedLevel.forward1:
                targetValue = detectionValues.forward1SpeedDetection;
                break;
            case SpeedLevel.forward2:
                targetValue = detectionValues.forward2SpeedDetection;
                break;
            case SpeedLevel.forward3:
                targetValue = detectionValues.forward3SpeedDetection;
                break;
            default:
                break;
        }

        targetValue += lightDetectionBonus;

        if (additionalDetection == 0 && !setUsingEditor)
        {
            LerpDetectionValue(targetValue);
        }

        if (MonsterStateMachine.Instance == null)
            return;

        float distanceToShip = MonsterStateMachine.Instance.GetDistanceToShip();
        if (currentDetectionMultiplier >= monsterMinimalDetectionValue && distanceToShip <= monsterMinimalDistanceFromShip)
        {
            time += Time.deltaTime;
            float changedTimer = timer - (currentDetectionMultiplier * 2);

            if (time >= changedTimer)
            {
                isSearching = false;
                time = 0;
            }

            if (!isSearching)
            {
                MonsterStateMachine.Instance.InvestigatingState.SetInvestigationRadius(currentDetectionMultiplier);
                MonsterStateMachine.Instance.SwitchState(MonsterStateMachine.Instance.InvestigatingState);
                isSearching = true;
            }
        }
    }

    void LerpDetectionValue(float targetDetectionValue)
    {
        float previousDetectionMultiplier = currentDetectionMultiplier;

        currentDetectionMultiplier = Mathf.Lerp(currentDetectionMultiplier, targetDetectionValue, Time.deltaTime * lerpTime);
        currentDetectionMultiplier = Mathf.Clamp(currentDetectionMultiplier, currentDetectionMultiplier, targetDetectionValue);
        currentDetectionMultiplier = Mathf.Round(currentDetectionMultiplier * 1000f) / 1000f;

#if UNITY_EDITOR
        Debug.Log($"current detection: {currentDetectionMultiplier} current speed level: {speedLevel}");
#endif
    }
}
