using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDangerVisualOnTop : MonoBehaviour
{
    [SerializeField] private Light[]  lights;

    private bool hasChanged;
    
    Coroutine timerCoroutine;
    
    void Start()
    {
        hasChanged = false;
        
        ShipDamage.Instance.OnDamageTaken += ShipDamage_OnDamageTaken;
        StartFire.OnFireStart += StartFire_OnFireStart;
    }

    private void ChangeColors()
    {
        if (hasChanged)
            return;
        
        Color originalColor  = lights[0].color;
        
        foreach (var light in lights)
        {
            light.color = Color.red;
        }
        hasChanged = true;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
            
        timerCoroutine = StartCoroutine(Timer(originalColor));
    }

    private void StartFire_OnFireStart()
    {
        ChangeColors();
    }

    private void ShipDamage_OnDamageTaken(object sender, int e)
    {
        ChangeColors();
    }
    
    IEnumerator Timer(Color originalColor)
    {
        yield return new WaitForSeconds(20f);
        hasChanged = false;
        
        foreach (var light in lights)
        {
            light.color = originalColor;
        }
        
    }
}
