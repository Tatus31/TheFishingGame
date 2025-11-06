using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrossBorder : MonoBehaviour
{
    public enum BorderType
    {
        NotDefined,
        TraderZone,
        StartingArea,
        Area1,
        Area2,
        Area3,
    }

    [SerializeField]
    private BorderType borderType = BorderType.NotDefined;

    private static float _lastCrossTime = -1f;
    
    private static readonly HashSet<BorderType> SentBorders = new HashSet<BorderType>();

    private void Start()
    {
        if (borderType == BorderType.NotDefined)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"BorderType not defined for {this.name}");
#endif
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Ship")) 
            return;
        
        if (SentBorders.Contains(borderType))
            return;
        
        float currentTime = Time.time;
        float travelTime = 0f;

        if (_lastCrossTime >= 0f)
        {
            travelTime = currentTime - _lastCrossTime;
        }

        _lastCrossTime = currentTime; 

        var analyticsData = new Dictionary<string, object>
        {
            { "crossedBorder", borderType.ToString() },
            { "travelTime", travelTime }
        };

        AnalyticsEvents.SendAnalyticsEvent("OnCrossedBorder", analyticsData);
        SentBorders.Add(borderType);
    }
}