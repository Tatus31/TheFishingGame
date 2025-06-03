using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygenLevel : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] float maxOxygen = 100f;
    [SerializeField] float oxygenDepletionRate = 10f;
    [SerializeField] float oxygenRefillRate = 20f; 

    bool isUnderwater = false;

    void Start()
    {
        slider.maxValue = maxOxygen;
        slider.value = maxOxygen;
    }

    void Update()
    {
        CheckIfUnderwater();
        UpdateOxygenOverTime();
    }

    void CheckIfUnderwater()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, float.MaxValue, waterLayer))
        {
            isUnderwater = true;
        }
        else
        {
            isUnderwater = false;
        }
    }

    void UpdateOxygenOverTime()
    {
        if (isUnderwater)
        {
            UpdateOxygenLevel(-oxygenDepletionRate * Time.deltaTime);
        }
        else
        {
            UpdateOxygenLevel(oxygenRefillRate * Time.deltaTime);
        }
    }

    public void UpdateOxygenLevel(float oxygenChange)
    {
        slider.value = Mathf.Clamp(slider.value + oxygenChange, 0, maxOxygen);
    }

    public bool IsUnderwater()
    {
        return isUnderwater;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isUnderwater ? Color.blue : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 100f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 10f);
    }
}