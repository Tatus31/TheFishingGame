using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ActivateSprinklers : MonoBehaviour
{
    [SerializeField] GameObject SprinklerVFXObj;
    [SerializeField] GameObject SprinklerCooldownVFXObj; 

    StartFire startFire;
    
    float sprinklerTime = 15f;
    float cooldownTime = 60f;
    
    bool isOnCooldown = false;

    private void Start()
    {
        if (SprinklerVFXObj == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SprinklerVFXObj is not assigned in the inspector.");
#endif
            return;
        }

        if (SprinklerCooldownVFXObj == null)
        {
#if UNITY_EDITOR
            Debug.LogError("SprinklerCooldownVFXObj is not assigned in the inspector.");
#endif
            return;
        }
        
        SprinklerCooldownVFXObj.SetActive(false);
        SprinklerVFXObj.SetActive(false);

        if (TryGetComponent<StartFire>(out StartFire startFire))
        {
            this.startFire = startFire;
        }
    }

    private void Update()
    {
        if (InputManager.Instance.GetSprinklersInputDown())
        {
            if (!isOnCooldown)
            {
                StartSprinklers();
            }
            else
            {
                if(!SprinklerCooldownVFXObj.activeSelf)
                    SprinklerCooldownVFXObj.SetActive(true);
#if UNITY_EDITOR
                Debug.Log("Sprinklers are cooling down.");
#endif
            }
        }
    }

    void StartSprinklers()
    {
        SprinklerCooldownVFXObj.SetActive(false);
        StartCoroutine(SprinklerRoutine());
    }

    private IEnumerator SprinklerRoutine()
    {
        isOnCooldown = true;

        var vfx = SprinklerVFXObj.GetComponent<VisualEffect>();
        SprinklerVFXObj.SetActive(true);
        vfx?.Play();
        AudioManager.PlaySound(AudioManager.SprinklerSound);

        yield return new WaitForSeconds(sprinklerTime);

        vfx?.Stop();
        startFire.FireActionStop();
        AudioManager.MuteSound(AudioManager.SprinklerSound);

        // yield return new WaitForSeconds(fadeOutDuration); 
        //SprinklerVFXObj.SetActive(false);
        
        if(!SprinklerCooldownVFXObj.activeSelf)
            SprinklerCooldownVFXObj.SetActive(true);
        
        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;
        
        if(SprinklerCooldownVFXObj.activeSelf)
            SprinklerCooldownVFXObj.SetActive(false);
#if UNITY_EDITOR        
        Debug.Log("Sprinklers are ready to use again.");
#endif
    }

}
