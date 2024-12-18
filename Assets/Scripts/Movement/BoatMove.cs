using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatMove : MonoBehaviour
{

    public float turnspeed = 1000f;
    public float accelerateSpeed = 1000f;
    public float maxLinearSpeed = 10f;   
    public float maxAngularSpeed = 10f;
    public string test;
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

        rb.AddTorque(0f, h * turnspeed * Time.deltaTime, 0f);
        rb.AddForce(transform.forward * v * accelerateSpeed * Time.deltaTime);
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
    }
}
