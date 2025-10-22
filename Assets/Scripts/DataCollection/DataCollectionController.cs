using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class DataCollectionController : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync(); 
        AnalyticsService.Instance.StartDataCollection(); 
        Debug.Log("Analytics initialized");
    }
}
