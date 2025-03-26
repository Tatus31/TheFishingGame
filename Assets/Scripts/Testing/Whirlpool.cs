using UnityEngine;

public class Whirlpool : MonoBehaviour
{
    [SerializeField] private Transform centerPoint; // Środek wiru
    [SerializeField] private float pullForce = 5f; // Siła przyciągania
    [SerializeField] private float rotationSpeed = 50f; // Prędkość obrotu w wirze


    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
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
}