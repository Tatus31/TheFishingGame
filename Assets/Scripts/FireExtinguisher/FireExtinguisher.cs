using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    public static FireExtinguisher Instance;

    [SerializeField] LayerMask layerMask;
    [SerializeField] int timeToPutOutFire;
    [SerializeField] int fireGracePeriodTime;
    [SerializeField] GameObject particleSystemObj;

    StartFire fire;

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
        particleSystemObj.SetActive(false);
    }

    private void Update()
    {
        if (InputManager.Instance.IsLeftMouseButtonHeld())
        {
            particleSystemObj.SetActive(true);

            if (MouseWorldPosition.GetInteractable(layerMask) && InputManager.Instance.IsLeftMouseButtonHeld())
            {
                StartCoroutine(StartFireExtinguisher());
            }
        }
        else
        {
            particleSystemObj.SetActive(false);
        }

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

            yield return new WaitForSeconds(timeToPutOutFire);
            fire.IsOnFire = false;
        }

        yield return null; 
    }
}
