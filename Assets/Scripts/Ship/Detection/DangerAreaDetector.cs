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

    [SerializeField] private float coolDownTime = 1f;
    private bool isOnCoolDown = false;

    [SerializeField] private float underWaterOffset = 0.2f;

    public static event EventHandler<DangerObject> OnEnterDangerArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            if (!isOnCoolDown)
            {
                Vector3 adjustedPosition = transform.position;
                adjustedPosition.y -= underWaterOffset;
                transform.position = adjustedPosition;

                OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, true));
                Debug.Log($"Entering Danger Area new ship position {transform.position}");

                isOnCoolDown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, false));
            Debug.Log("Exiting Danger Area");
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
