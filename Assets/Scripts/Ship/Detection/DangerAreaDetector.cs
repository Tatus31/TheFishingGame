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

    public static event EventHandler<DangerObject> OnEnterDangerArea;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            Debug.Log("Entering Danger Area");
            OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("DangerArea"))
        {
            Debug.Log("Exiting Danger Area");
            OnEnterDangerArea?.Invoke(this, new DangerObject(this.transform, false));
        }
    }
}
