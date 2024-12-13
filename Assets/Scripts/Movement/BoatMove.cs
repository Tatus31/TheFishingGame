using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMove : MonoBehaviour
{

    public float turnspeed = 1000f;
    public float accelerateSpeed = 1000f;
    public string test;

    private Rigidbody rb;

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
        Vector3 lateralVelocity = Vector3.Dot(rb.velocity, transform.right) * transform.right;
        rb.AddForce(-lateralVelocity * 100f, ForceMode.Acceleration);
    }
}
