using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMove : MonoBehaviour
{

    public float turnspeed = 1000f;
    public float accelerateSpeed = 100f;
    public float maxLinearSpeed = 10f;   
    public float maxAngularSpeed = 10f;
    public string test;
    public float brakeForce = 5f;
   // public float decelerationRate = 0.95f;
    //public InputAction boatcontroller;
    private Rigidbody rb;

    //private void OnEnable()
    //{
        //boatcontroller.Enable();
    //}

    //private void OnDisable()
    //
       // boatcontroller.Disable();
    //}

    // Start is called before the first frame update
    void Start()
    {   

        rb = GetComponent<Rigidbody>();
        this.enabled = false;
    }
    
    

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isHori = true;

        rb.AddTorque(0f, h * turnspeed * Time.deltaTime, 0f);
        rb.AddForce(transform.forward * v * accelerateSpeed * Time.deltaTime);

        if (isHori == true)
        {
            rb.AddForce(transform.forward / 10000000000, ForceMode.Force);
            Console.WriteLine("jak dziala to git");
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Zakładamy, że Space to przycisk hamowania
        {
            ApplyBrakes();
        }
    }

    private void FixedUpdate()
    {
        // Ogranicz prędkość liniową
        if (rb.velocity.magnitude > maxLinearSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxLinearSpeed;
        }

        // Ogranicz prędkość kątową (obrotu)
        if (rb.angularVelocity.magnitude > maxAngularSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularSpeed;
            
        }
        
        
        Vector3 lateralVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
        rb.AddForce(-lateralVelocity * 100f, ForceMode.Acceleration);

        if (rb.velocity.magnitude > 0)
        {
            Vector3 dragForce = -rb.velocity.normalized * rb.velocity.magnitude * 0.5f;
            rb.AddForce(dragForce, ForceMode.Force);
        }
    }

    void ApplyBrakes()
    {
       

        // Zatrzymanie jeśli prędkość jest bardzo niska
        if (rb.velocity.magnitude > 0.1f)
        {
            // Oblicz wektor przeciwny do ruchu
            Vector3 reverseForce = -rb.velocity.normalized * brakeForce;

            // Zastosuj siłę
            rb.AddForce(reverseForce, ForceMode.Force);
        }
    }
}
