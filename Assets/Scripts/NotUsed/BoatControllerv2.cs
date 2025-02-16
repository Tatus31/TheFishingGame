using UnityEngine;

public class BoatControllerv2 : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("Movement Settings")]
    public float motorTorque = 1500f; // Siła napędowa na koła
    public float maxSteerAngle = 30f; // Maksymalny kąt skrętu kół
    public float brakeForce = 3000f;  // Siła hamowania

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;

    void Update()
    {
        // Pobierz wejścia gracza
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);

        // Sterowanie łódką
        Steer();
        Accelerate();
        ApplyBrakes();
    }

    void Steer()
    {
        // Oblicz kąt skrętu dla przednich kół
        float steerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;
    }

    void Accelerate()
    {
        // Nadaj moment obrotowy na wszystkie koła
        float torque = motorTorque * verticalInput;
        frontLeftWheel.motorTorque = torque;
        frontRightWheel.motorTorque = torque;
        rearLeftWheel.motorTorque = torque;
        rearRightWheel.motorTorque = torque;
    }

    void ApplyBrakes()
    {
        // Zastosuj siłę hamowania
        float brake = isBraking ? brakeForce : 0f;
        frontLeftWheel.brakeTorque = brake;
        frontRightWheel.brakeTorque = brake;
        rearLeftWheel.brakeTorque = brake;
        rearRightWheel.brakeTorque = brake;
    }
}