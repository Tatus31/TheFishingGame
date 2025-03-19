using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using Random = UnityEngine.Random;

public class MoveClouds : MonoBehaviour
{
    [SerializeField] private Transform Cloud;
    [SerializeField] private GameObject CloudGameObject;
    [SerializeField] private GameObject CloudTowardsPoint;
    [SerializeField] private List<Transform> CloudTransforms = new List<Transform>();

    public List<Transform> GetCloudTransformsList
    {
        get { return CloudTransforms; }
        set { CloudTransforms = value; }
    }
    
    private void Update()
    {
        MoveCloud(0.05f);
    }
    
    void MoveCloud(float moveamount)
    {
        CloudGameObject.gameObject.transform.position = Vector3.MoveTowards(CloudGameObject.gameObject.transform.position,CloudTowardsPoint.gameObject.transform.position, moveamount);
        
    }

    
}
