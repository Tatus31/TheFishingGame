using UnityEngine;

public class GlobalCooldown : MonoBehaviour
{
    public static GlobalCooldown Instance { get; private set; }

    float cooldownTimer = 0f;
    bool isInCooldown = false;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;
    }

    private void Update()
    {
        if (isInCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isInCooldown = false;
            }
        }
    }

    public void StartCooldown(float duration)
    {
        cooldownTimer = duration;
        isInCooldown = true;
    }

    public bool IsInCooldown()
    {
        return isInCooldown;
    }

    public float GetRemainingCooldown()
    {
        return Mathf.Max(0, cooldownTimer);
    }
}