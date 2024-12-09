using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{   

    private Rigidbody pbMovement;
    public bool IsOnboat = false;
    public float speed = 5f;
    public float rotationSpeed = 50f;


    // Start is called before the first frame update
    void Start()
    {
        pbMovement = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOnboat)
        {
            // Przykład prostego sterowania
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveX, 0, moveZ);

            pbMovement.MovePosition(transform.position + movement * Time.deltaTime);


        }


        if (IsPlayerControlling() && Input.GetKeyDown(KeyCode.E))
        {
            Transform player = transform.GetChild(0); // Zakładamy, że gracz jest dzieckiem łódki
            ExitBoat();
        }
    }

    public void EnterBoat(GameObject boat)
    {
        IsOnboat = true;
        transform.parent = boat.transform; // Ustawienie rodzica (gracz "przykleja się" do łódki)
        pbMovement.isKinematic = true; // Wyłączenie fizyki gracza
        this.gameObject.SetActive(false); // Ukrycie gracza (opcjonalne, jeśli sterujemy tylko łódką)
    }

    public void ExitBoat()
    {
        IsOnboat = false;
        transform.parent = null; // Odłączenie od łódki
        pbMovement.isKinematic = false; // Włączenie fizyki
        this.gameObject.SetActive(true); // Pokazanie gracza
    }

    void HandleBoatMovement()
    {
        // Sterowanie łódką
        float move = Input.GetAxis("Vertical");
        float rotation = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.forward * move * speed * Time.deltaTime);
        transform.Rotate(Vector3.up, rotation * rotationSpeed * Time.deltaTime);
    }

    bool IsPlayerControlling()
    {
        // Logika sprawdzania (np. czy gracz wszedł na łódkę)
        return transform.childCount > 0; // Jeśli łódka ma dziecko (gracza jako dziecko)
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitBoat();
        }
    }

}
