using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
   public Rigidbody body;
    public float depthBeforeSubmerged = 1f; //okresla jak gleboko moze zanurzyc sie obiekt za nim zacznie dzialac na niego sila wyporu
    public float displacementAmount = 3f; // tu ustalamy wartosc sily wyporu
    public int floaterCount = 1; // ile floaterow ktore np. aplikuja na obiekt sile wyporu jest
    public float waterDrag = 0.99f; // jak duzy opor woda naklada na obiekt
    public float waterAngularDrag = 0.5f; // to samo co zwykly drag tylko  ze katowe

    private void FixedUpdate()
    {
        body.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration); // aplikuje grawitacje na obiekt poprzez floatery
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.x); // wysokosc fali
        if (transform.position.y < waveHeight) // sprawdzamy czy obiekt jest nizej niz powierzchnia wody
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight-transform.position.y) / depthBeforeSubmerged) * displacementAmount; // obliczamy jaka czesc obiektu jest zanuzona
            body.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier,0f),transform.position,ForceMode.Acceleration); // nadaje sile do gory ktora kontruje sile grawitacji dzialajaca na obiekt
            body.AddForce(displacementMultiplier * -body.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange); // redukuje predkosc obiektu na podstawie drag symulujac opor wody
            body.AddTorque(displacementMultiplier * -body.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange); // to samo co linije wyzej tylko ze katowa
        }
    }


}
