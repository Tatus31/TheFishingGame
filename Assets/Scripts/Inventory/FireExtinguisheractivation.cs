using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class FireExtinguisheractivation : MonoBehaviour
{
    public ParticleSystem particle;
    public Camera FPCamera;
    Player player;
    ItemObject item;


    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
       Activate();
       HitFire();
    }

    public void Activate()
    {

        for (int i = 0; i < player.equipment.GetSlots.Length; i++)
        {
            var itemType = player.equipment.GetSlots[i];

            for (int j = 0; j < itemType.AllowedItems.Length; j++)
            {

                if (itemType.AllowedItems[j] == ItemType.fireExtinguisher)
                {
                    if (itemType.item.id == -1)
                    {
                        return;
                    }

                    if(itemType.item.id == 11)
                    {
                        if (Input.GetKey(KeyCode.N))
                        {
                            if (particle != null)
                            {
                                particle.Play();
                                Debug.Log("Naciskam N");

                                HitFire();
                            }
                        }
                        else if (Input.GetKey(KeyCode.K))
                        {
                            particle.Stop();
                            Debug.Log("Naciskam K");
                        }
                    }

                }
            }
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
