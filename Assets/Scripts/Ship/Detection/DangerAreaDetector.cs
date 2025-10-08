using System;
using UnityEngine;

public class DangerAreaDetector : MonoBehaviour
{
    public class DangerObject
    {
        public Transform transform;
        public bool hasEnteredDangerousArea;


        public DangerObject(Transform transform, bool hasEnteredDangerousArea)
        {
            this.transform = transform;
            this.hasEnteredDangerousArea = hasEnteredDangerousArea;
        }
    }

    float coolDownTime = 1f;
    bool isOnCoolDown = false;

    public static event EventHandler<DangerObject> OnEnterDangerArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            if (!isOnCoolDown)
            {
                OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, true));
                Debug.Log($"entering Danger Area sending {this.transform.position}");
                isOnCoolDown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, false));
            Debug.Log("exiting Danger Area");
        }
    }

    private void Update()
    {
        if (coolDownTime > 0)
        {
            coolDownTime -= Time.deltaTime;
        }
        else
        {
            coolDownTime = 1f;
            isOnCoolDown = false;
        }
    }
}
