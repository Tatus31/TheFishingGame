using System;
using UnityEngine;

public class ShipDamage : MonoBehaviour
{
    public static ShipDamage Instance;

    public event EventHandler OnSinkingShipByDamage;
    public event EventHandler<int> OnDamageTaken;

    Ship ship;
    ShipMovement shipMovement;
    Vector3 flatVel;

    int currentHealth;
    int maxHealth;

    [Header("Damage values")]
    [Tooltip("Damage done when the ship collides with dangers")]
    [SerializeField] int baseCollisionDamage = 100;
    [Tooltip("Damage done when the ship enters a toxic area")]
    [SerializeField] int baseToxicDamage = 2;
    [Tooltip("Damage done when the ship is on fire (TickRate)")]
    [SerializeField] int baseFireDamage = 5;

    [Header("Monster Damage by Type")]
    [Tooltip("Damage done when the ship is hit by a large monster")]
    [SerializeField] int baseLargeMonsterDamage = 50;
    [Tooltip("Damage done when the ship is hit by a medium monster")]
    [SerializeField] int baseMediumMonsterDamage = 25;
    [Tooltip("Damage done when the ship is hit by a small monster")]
    [SerializeField] int baseSmallMonsterDamage = 10;

    [Header("Damage Cooldown")]
    [Tooltip("Time the ship is invulnerable after taking damage")]
    [SerializeField] float damageCooldownDuration = 0.5f;

    bool isInDamageCooldown = false;
    bool isInvincible;
    float cooldownTimeRemaining = 0f;

    public int PreviousHealth { get; private set; }
    public int BaseFireDamage { get { return baseFireDamage; } private set { baseFireDamage = value; } }
    public bool IsInvulnerable { get { return isInDamageCooldown; } }
    public bool IsInvincible { get { return isInvincible; } set { isInvincible = value; } }

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
        ship = GetComponent<Ship>();
        shipMovement = GetComponent<ShipMovement>();

        shipMovement.OnShipSpeedChange += ShipMovement_OnShipSpeedChange;
        Ship.OnStatsChange += Ship_OnStatsChange;

        currentHealth = ship.GetModifiedStatValue(Stats.Health);
        maxHealth = ship.GetModifiedStatValue(Stats.Health);
    }

    private void Update()
    {
        if (isInvincible)
            return;

        if (isInDamageCooldown)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            if (cooldownTimeRemaining <= 0f)
            {
                EndDamageCooldown();
            }
        }
    }

    private void Ship_OnStatsChange(object sender, EventArgs e)
    {
        currentHealth = ship.GetModifiedStatValue(Stats.Health);
        maxHealth = ship.GetModifiedStatValue(Stats.Health);

        SetPermanentSavedBaseStatValue(Stats.Health);
        SetPermanentSavedModifiedStatValue(Stats.Health);
    }

    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
#if UNITY_EDITOR
        Debug.Log($"restored {amount} health");
#endif
        UpdateAttributes();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isInvincible)
            return;

        if (isInDamageCooldown)
            return;

        if (collision.collider.CompareTag(TagHolder.danger))
        {
            TakeCollisionDamage(baseCollisionDamage);
        }

        if (collision.collider.CompareTag(TagHolder.monsterDanger))
        {
            DetectionManager.MonsterType monsterType = GetMonsterTypeFromCollision(collision);
            TakeMonsterDamage(monsterType);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isInvincible)
            return;

        if (other.CompareTag(TagHolder.toxicDanger))
        {
            TakeDamage(baseToxicDamage);
        }
    }

    DetectionManager.MonsterType GetMonsterTypeFromCollision(Collision collision)
    {
        Transform monsterTransform = collision.transform;
        if (DetectionManager.Instance != null)
        {
            DetectionManager.MonsterType type = DetectionManager.Instance.GetMonsterType(monsterTransform);
            if (type != DetectionManager.MonsterType.NotRecognized)
            {
                return type;
            }
            if (monsterTransform.parent != null)
            {
                type = DetectionManager.Instance.GetMonsterType(monsterTransform.parent);
                if (type != DetectionManager.MonsterType.NotRecognized)
                {
                    return type;
                }
            }

            foreach (Transform child in monsterTransform.GetComponentsInChildren<Transform>())
            {
                type = DetectionManager.Instance.GetMonsterType(child);
                if (type != DetectionManager.MonsterType.NotRecognized)
                {
                    return type;
                }
            }
        }

        if (monsterTransform.GetComponentInParent<MonsterLargeStateMachine>() != null)
        {
            return DetectionManager.MonsterType.Large;
        }
        else if (monsterTransform.GetComponentInParent<MediumMonsterStateMachine>() != null)
        {
            var mediumStateMachine = monsterTransform.GetComponentInParent<MediumMonsterStateMachine>();
            return mediumStateMachine.isSmallMonster ? DetectionManager.MonsterType.Small : DetectionManager.MonsterType.Medium;
        }

        return DetectionManager.MonsterType.Medium;
    }

    public void TakeMonsterDamage(DetectionManager.MonsterType monsterType)
    {
        if (isInvincible || isInDamageCooldown)
            return;

        int damageAmount = GetMonsterDamageByType(monsterType);

#if UNITY_EDITOR
        Debug.Log($"Ship hit by {monsterType} monster, taking {damageAmount} damage");
#endif

        if(StickToShip.Instance.IsOnShip)
            CameraEffectsManager.Instance.SwitchState(CameraEffectsManager.Instance.colisionShakeState);

        TakeDamage(damageAmount);
    }

    private int GetMonsterDamageByType(DetectionManager.MonsterType monsterType)
    {
        switch (monsterType)
        {
            case DetectionManager.MonsterType.Large:
                return baseLargeMonsterDamage;
            case DetectionManager.MonsterType.Medium:
                return baseMediumMonsterDamage;
            case DetectionManager.MonsterType.Small:
                return baseSmallMonsterDamage;
            default:
                return baseMediumMonsterDamage; 
        }
    }

    void TakeCollisionDamage(int baseDamage)
    {
        if (isInvincible)
            return;

        if (isInDamageCooldown)
            return;

        PreviousHealth = currentHealth;
        float maxSpeed = shipMovement.MaxSpeed;
        float speedRatio = Mathf.Floor(flatVel.magnitude) / maxSpeed;
        int actualDamage = 0;

        if (speedRatio > 0)
        {
            float damageMultiplier = 0.5f + (speedRatio * 1.5f);
            actualDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        }
#if UNITY_EDITOR
        Debug.Log($"ship took {actualDamage} collision damage");
#endif
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);
        OnDamageTaken?.Invoke(this, actualDamage);
        UpdateAttributes();

        if (StickToShip.Instance.IsOnShip)
            CameraEffectsManager.Instance.SwitchState(CameraEffectsManager.Instance.colisionShakeState);

        StartDamageCooldown();
    }

    public void TakeDamage(int baseDamage)
    {
        if (isInvincible)
            return;

        if (isInDamageCooldown)
            return;

        PreviousHealth = currentHealth;
        int actualDamage = baseDamage;

        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);
        OnDamageTaken?.Invoke(this, actualDamage);
        UpdateAttributes();

        StartDamageCooldown();
    }

    void StartDamageCooldown()
    {
        isInDamageCooldown = true;
        cooldownTimeRemaining = damageCooldownDuration;
    }

    void EndDamageCooldown()
    {
        isInDamageCooldown = false;
#if UNITY_EDITOR
        Debug.Log("Ship is vulnerable again");
#endif
    }

    void UpdateAttributes()
    {
        foreach (var attribute in ship.objectAttributes)
        {
            if (attribute.type == Stats.Health)
            {
                attribute.Value.SetModifiedValueDirectly(currentHealth);
                if (currentHealth == 0)
                {
                    OnSinkingShipByDamage?.Invoke(this, EventArgs.Empty);
                }
                break;
            }
        }
    }

    void ShipMovement_OnShipSpeedChange(object sender, Vector3 e)
    {
        flatVel = e;
    }

    private void OnDestroy()
    {
        if (shipMovement != null)
        {
            shipMovement.OnShipSpeedChange -= ShipMovement_OnShipSpeedChange;
        }
    }

    public int GetModifiedStatValue(Stats stats) => ship.GetModifiedStatValue(stats);

    public int GetSavedModifiedValue(Stats stats) => ship.GetSavedModifiedStatValue(stats);

    public int GetPermanentModifiedStatValue(Stats stats) => ship.GetPermanentSavedModifiedStatValue(stats);

    public int GetPermanentSavedStatValue(Stats stats) => ship.GetPermanentSavedBaseStatValue(stats);

    public void SetPermanentSavedModifiedStatValue(Stats stats) => ship.SetPermanentSavedModifiedStatValue(stats);

    public void SetPermanentSavedBaseStatValue(Stats stats) => ship.SetPermanentSavedBaseStatValue(stats);

    public int GetLargeMonsterDamage() => baseLargeMonsterDamage;
    public int GetMediumMonsterDamage() => baseMediumMonsterDamage;
    public int GetSmallMonsterDamage() => baseSmallMonsterDamage;
}