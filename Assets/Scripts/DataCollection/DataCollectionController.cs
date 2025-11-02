using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public static class AnalyticsEvents
{
    public static void SendAnalyticsEvent(string eventName, Dictionary<string, object> data)
    {
        var evt = new CustomEvent(eventName);

        foreach (var kvp in data)
        {
            evt.Add(kvp.Key, kvp.Value);
        }

        AnalyticsService.Instance.RecordEvent(evt);
        AnalyticsService.Instance.Flush();

        if (DataCollectionController.IsCollectingData)
        {
#if UNITY_EDITOR
            Debug.Log($"Sent analytics event: {eventName}");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Data collection is disabled. Event {eventName} was not sent.");
#endif
        }
    }

}

public class DataCollectionController : MonoBehaviour
{
    public static DataCollectionController Instance;

    bool isCollectingData = false;

    public static bool IsCollectingData { get { return Instance.isCollectingData; } }

    void Awake()
    {
        if (Instance != null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
#endif
        }
        Instance = this;
    }

    async void Start()
    {
        await UnityServices.InitializeAsync(); 
        //AnalyticsService.Instance.StartDataCollection(); 
        //Debug.Log("Analytics initialized");
    }

    public void StartCollectingData()
    {
        AnalyticsService.Instance.StartDataCollection();
        isCollectingData = true;
#if UNITY_EDITOR
        Debug.Log("Data collection started");
#endif
    }

    public void StopCollectingData()
    {
        AnalyticsService.Instance.StopDataCollection();
        isCollectingData = false;
#if UNITY_EDITOR
        Debug.Log("Data collection stopped");
#endif
    }
}