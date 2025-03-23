using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class MoveClouds : MonoBehaviour
{
    [SerializeField] private Transform Cloud;
    [SerializeField] private GameObject CloudGameObject;
    [SerializeField] private GameObject CloudTowardsPoint;
    [SerializeField] private List<Transform> CloudTransforms = new List<Transform>();
    public float speed = 1f; // Szybkość ruchu
    private float t = 0f; // Pozycja na splajnie (0 - początek, 1 - koniec)
    private int direction = 1; // Kierunek: 1 = przód, -1 = tył
    public SplineContainer spline;

    public List<Transform> GetCloudTransformsList
    {
        get { return CloudTransforms; }
        set { CloudTransforms = value; }
    }
    
    private void Update()
    {
        if (spline == null || spline.Spline == null) return;

        t += Time.deltaTime * speed * direction; // Ruch w aktualnym kierunku

        // Odbijanie się na końcach
        if (t >= 1f)
        {
            t = 1f; // Zatrzymanie na końcu
            direction = -1; // Zmiana kierunku na przeciwny
        }
        else if (t <= 0f)
        {
            t = 0f; // Zatrzymanie na początku
            direction = 1; // Zmiana kierunku na przeciwny
        }

        Cloud.transform.position = spline.EvaluatePosition(t);
    }
    
    void MoveCloud(float moveamount)
    {
        CloudGameObject.gameObject.transform.position = Vector3.MoveTowards(CloudGameObject.gameObject.transform.position,CloudTowardsPoint.gameObject.transform.position, moveamount);
        
    }

    
}
