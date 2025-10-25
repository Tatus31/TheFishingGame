using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrossBorder : MonoBehaviour
{
    public enum BorderType
    {
        NotDefined,
        TraderZone,
        StartingArea
    }

    [SerializeField]
    BorderType borderType = BorderType.NotDefined;

    private void Start()
    {
        if (borderType == BorderType.NotDefined)
        {
            Debug.LogWarning($"BorderType not defined for {this.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Ship>(out Ship ship))
        {
            var analiticsData = new Dictionary<string, object>
            {
                { "crossedBorder", borderType.ToString() }
            };

            AnalyticsEvents.SendAnalyticsEvent("OnCrossedBorder", analiticsData);
        }
    }
}
