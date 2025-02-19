using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDamage : MonoBehaviour
{
    public static ShipDamage Instance;

    public event EventHandler<int> OnDamageTaken;

    public int PreviousHealth { get; private set; }

    Ship ship;
    ShipMovement shipMovement;

    Vector3 flatVel;

    int currentHealth;
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
    }

    private void Update()
    {
        //Debug.Log(Mathf.Floor(flatVel.magnitude));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Danger"))
        {
            TakeDamage(baseDamage);
        }
    }

    void TakeDamage(int baseDamage)
    {
        PreviousHealth = currentHealth;
        currentHealth = ship.GetModifiedStatValue(Stats.Health);
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

    void UpdateAttributes()
    {
        foreach (var attribute in ship.objectAttributes)
        {
            if (attribute.type == Stats.Health)
            {
                attribute.value.ModifiedValue = currentHealth;
                break;
            }
        }
    }

    void ShipMovement_OnShipSpeedChange(object sender, Vector3 e)
    {
        flatVel = e;
    }

}