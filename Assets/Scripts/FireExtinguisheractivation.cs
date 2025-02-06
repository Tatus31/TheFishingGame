using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class FireExtinguisheractivation : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Inventory inv;
    public Item it;
    private InventorySlot InventorySlot1;
    public Camera FPCamera;
    private SnapObj SnapObj;
    public Transform snappingpoint;
    public GameObject item2;


    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    

    private void Update()
    {

       Activate();
       HitFire();

    }

    public void Activate()
    {   

        //particleSystem.gameObject.SetActive(false);
        float distance2 = Vector3.Distance(item2.transform.position, snappingpoint.position);
        if (distance2 == 0)
        {
            if (Input.GetKey(KeyCode.N))
            {
                if (particleSystem != null)
                {
                    particleSystem.Play();
                    Debug.Log("Naciskam N");
                    
                    HitFire();
                }
            }
            else if (Input.GetKey(KeyCode.K))
            {
                particleSystem.Stop();
                Debug.Log("Naciskam K");
            }

        }
        else
        {
            Debug.Log("Nie dziala");
        }
        
        
        
        
    }

    public void HitFire()
    {
            // Sprawdza, czy naciśnięto klawisz "E"
        
            RaycastHit hit; // Zmienna do przechowywania informacji o trafieniu
            Ray ray = FPCamera.ScreenPointToRay(Input.mousePosition); // Tworzy promień z pozycji myszy

            if (Physics.Raycast(ray, out hit)) // Sprawdza, czy promień trafił w obiekt
            {
                if (hit.collider.gameObject.name == "Fire")
                {
                    Debug.Log("Patrzysz na obiekt: " + hit.collider.gameObject.name); // Wypisuje nazwę obiektu    
                }
                
            }
        
    }
    
}
