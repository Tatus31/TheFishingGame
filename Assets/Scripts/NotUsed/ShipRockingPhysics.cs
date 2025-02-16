using UnityEngine;

public class ShipRockingPhysics : MonoBehaviour
{
    public Rigidbody shipRigidbody;
    public float rockingForce = 10.0f; // Siła bujania
    public float rockingFrequency = 1.0f; // Częstotliwość bujania
    public float dampingFactor = 0.99f; // Tłumienie ruchu

    private float time;

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        // Dodanie siły momentu obrotowego do Rigidbody
        float torque = Mathf.Sin(time * rockingFrequency) * rockingForce;
        shipRigidbody.AddTorque(transform.forward * torque, ForceMode.Force);

        // Tłumienie rotacji (aby bujanie nie wymknęło się spod kontroli)
        shipRigidbody.angularVelocity *= dampingFactor;
    }
}
