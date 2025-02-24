using System;
using UnityEngine;

public class ShipDamage : MonoBehaviour
{
    public static ShipDamage Instance;

    public event EventHandler OnSinkingShipByDamage;
    public event EventHandler<int> OnDamageTaken;
    public int PreviousHealth { get; private set; }
    Ship ship;
    ShipMovement shipMovement;
    Vector3 flatVel;

    public int currentHealth;
    public int maxHealth;

    [SerializeField] int baseDamage = 100;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
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
        Debug.Log($"restored {amount} health");
        UpdateAttributes();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Danger"))
        {
            TakeCollisionDamage(baseDamage);
        }
    }

    void TakeCollisionDamage(int baseDamage)
    {
        PreviousHealth = currentHealth;
        float maxSpeed = shipMovement.MaxSpeed;
        float speedRatio = Mathf.Floor(flatVel.magnitude) / maxSpeed;
        int actualDamage = 0;

        if (speedRatio > 0)
        {
            float damageMultiplier = 0.5f + (speedRatio * 1.5f);
            actualDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
        }

        Debug.Log($"ship took {actualDamage} damage");
        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);
        OnDamageTaken?.Invoke(this, actualDamage);
        UpdateAttributes();
    }

    public void TakeFireDamage()
    {
        PreviousHealth = currentHealth;
        int actualDamage = 5;

        currentHealth -= actualDamage;
        currentHealth = Mathf.Max(0, currentHealth);
        OnDamageTaken?.Invoke(this, actualDamage);
        UpdateAttributes();
    }

    void UpdateAttributes()
    {
        foreach (var attribute in ship.objectAttributes)
        {
            if (attribute.type == Stats.Health)
            {
                attribute.Value.SetModifiedValueDirectly(currentHealth);
                if(currentHealth == 0)
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
}