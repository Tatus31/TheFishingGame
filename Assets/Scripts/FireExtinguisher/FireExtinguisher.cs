using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    public static FireExtinguisher Instance;

    [SerializeField] LayerMask layerMask;
    [SerializeField] int timeToPutOutFire;
    [SerializeField] int fireGracePeriodTime;

    StartFire fire;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");

        Instance = this;
    }

    private void Update()
    {
        if(MouseWorldPosition.GetInteractable(layerMask) && InputManager.Instance.IsLeftMouseButtonHeld())
            StartCoroutine(StartFireExtinguisher());
    }

    IEnumerator StartFireExtinguisher()
    {
        var fireObj = MouseWorldPosition.GetObjectOverMouse(layerMask);

        if(fireObj != null)
        {
            fire = fireObj.GetComponentInParent<StartFire>();
        }

        if (fire.IsOnFire)
        {
            Debug.Log("ptting out fire");
            yield return new WaitForSeconds(timeToPutOutFire);
            fire.IsOnFire = false;
        }

        yield return null; 
    }

    IEnumerator FireGracePeriod()
    {
        yield return new WaitForSeconds(timeToPutOutFire);
    }
}
