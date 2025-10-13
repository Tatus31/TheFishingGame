using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ActivateSprinklers : MonoBehaviour
{
    [SerializeField]
    GameObject SprinklerVFXObj;

    StartFire startFire;
    float sprinklerTime = 5f;
    float cooldownTime = 60f;
    bool isOnCooldown = false;

    private void Start()
    {
        if (SprinklerVFXObj == null)
        {
            Debug.LogError("SprinklerVFXObj is not assigned in the inspector.");
            return;
        }

        SprinklerVFXObj.SetActive(false);

        if (TryGetComponent<StartFire>(out StartFire startFire))
        {
            this.startFire = startFire;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!isOnCooldown)
            {
                StartSprinklers();
            }
            else
            {
                Debug.Log("Sprinklers are cooling down.");
            }
        }
    }

    void StartSprinklers()
    {
        StartCoroutine(SprinklerRoutine());
    }

    private IEnumerator SprinklerRoutine()
    {
        isOnCooldown = true;

        var vfx = SprinklerVFXObj.GetComponent<VisualEffect>();
        SprinklerVFXObj.SetActive(true);
        vfx?.Play();

        yield return new WaitForSeconds(sprinklerTime);

        vfx?.Stop();
        startFire.FireActionStop();

        SprinklerVFXObj.SetActive(false);

        yield return new WaitForSeconds(cooldownTime);

        isOnCooldown = false;
        Debug.Log("Sprinklers are ready to use again.");
    }
}
