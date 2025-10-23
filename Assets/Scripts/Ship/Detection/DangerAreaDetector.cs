using System;
using UnityEngine;

public class DangerAreaDetector : MonoBehaviour
{
    public class DangerObject
    {
        public Transform transform;
        public Vector3 adjustedPosition;
        public bool hasEnteredDangerousArea;

        public DangerObject(Transform transform, Vector3 adjustedPosition, bool hasEnteredDangerousArea)
        {
            this.transform = transform;
            this.adjustedPosition = adjustedPosition;
            this.hasEnteredDangerousArea = hasEnteredDangerousArea;
        }
    }


    [SerializeField] private float coolDownTime = 1f;
    private bool isOnCoolDown = false;

    [SerializeField] private float underWaterOffset = 0.2f;

    public static event EventHandler<DangerObject> OnEnterDangerArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerArea") && !isOnCoolDown)
        {
            Vector3 adjustedPosition = transform.position;
            adjustedPosition.y -= underWaterOffset;

            OnEnterDangerArea?.Invoke(
                this,
                new DangerObject(transform, adjustedPosition, true)
            );

            isOnCoolDown = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            OnEnterDangerArea?.Invoke(
                this,
                new DangerObject(transform, transform.position, false)
            );
            Debug.Log("Exiting Danger Area");
        }
    }

    private void Update()
    {
        if (coolDownTime > 0) coolDownTime -= Time.deltaTime;
        else
        {
            coolDownTime = 1f;
            isOnCoolDown = false;
        }
    }
}
