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
    
    

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        Activate();
        
    }

    public void Activate()
    {
        it.id = 0;
        it.name = "Gasnica";
        if (it.id == 0)
        {
            if(Input.GetKey(KeyCode.N))
            {
                if (particleSystem != null)
                {
                    particleSystem.Play();
                    Debug.Log("Naciskam N");
                    HitFire();
                }   
            }
            else if(Input.GetKey(KeyCode.K))
            {
                particleSystem.Stop();
                Debug.Log("Naciskam K");
            }
        }
        else
        {
            return;
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
