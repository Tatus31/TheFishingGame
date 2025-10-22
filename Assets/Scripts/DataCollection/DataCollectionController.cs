using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class DataCollectionController : MonoBehaviour
{
    public static DataCollectionController Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"there exists a {Instance.name} in the scene already");
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
        Debug.Log("Data collection started");
    }

    public void StopCollectingData()
    {
        AnalyticsService.Instance.StopDataCollection();
        Debug.Log("Data collection stopped");
    }
}
