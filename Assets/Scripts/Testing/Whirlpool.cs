using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Whirlpool : MonoBehaviour
{
    [SerializeField] private Transform centerPoint; // Środek wiru
    [SerializeField] private float pullForce = 5f; // Siła przyciągania
    [SerializeField] private float rotationSpeed = 50f; // Prędkość obrotu w wirze
    [SerializeField] private Transform EndPoint;
    [SerializeField] private LayerMask _layerMask;
    //private LineRenderer line;

    private void Start()
    {
       
    }

    private void Update()
    {
        //HandleWhirpoolRockDetection();
    }

    


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ship"))
        {   
            Debug.Log("Jestem w srodku");
            HandleWhirpoolRockDetection();
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Kierunek do środka wiru
                Vector3 direction = (centerPoint.position - other.transform.position).normalized;
                
                // Przyciąganie w stronę środka wiru
                rb.AddForce(direction * pullForce);
                
                // Obracanie statku wokół środka wiru
                rb.AddTorque(Vector3.up * rotationSpeed,ForceMode.Force);
            }
        }
    }

    void HandleWhirpoolRockDetection()
    {
        if (Physics.Raycast(EndPoint.transform.position,EndPoint.transform.forward,out RaycastHit hitinfo,100,_layerMask) == true)
        {
            Debug.DrawRay(EndPoint.transform.position,EndPoint.transform.forward * 20,Color.green);
            Debug.Log("Hit Something!");
            pullForce = 350f;
            rotationSpeed = 200f;
        }
        else
        {
            Debug.DrawRay(EndPoint.transform.position,EndPoint.transform.forward * 20,Color.red);
            Debug.Log("Not hitting anything!");
            pullForce = 700f;
            rotationSpeed = 677f;
        }
    }
    
        
}