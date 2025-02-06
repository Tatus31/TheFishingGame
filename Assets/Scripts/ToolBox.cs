using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToolBox : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;

    [SerializeField]
    private Texture texture;
    [SerializeField]
    private Texture texture2;

    private Renderer renderer;

    [SerializeField]
    private Camera FPCamera;

    private void Update()
    {   

        ChangeCubeTexture();
        IsHeLooking();
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        cube = GetComponent<GameObject>();
    }

    private void ChangeCubeTexture()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Debug.Log("Najpierw to");
            renderer.material.mainTexture = texture;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Pozniej to");
            renderer.material.mainTexture = texture2;
        }
        
    }

    private void IsHeLooking()
    {
        // Sprawdza, czy naciśnięto klawisz "E"

        RaycastHit hit; // Zmienna do przechowywania informacji o trafieniu
        Ray ray = FPCamera.ScreenPointToRay(Input.mousePosition); // Tworzy promień z pozycji myszy

        if (Physics.Raycast(ray, out hit)) // Sprawdza, czy promień trafił w obiekt
        {
            if (hit.collider.gameObject.name == "ScianaStatku")
            {
                Debug.Log("Patrzysz na obiekt: " + hit.collider.gameObject.name); // Wypisuje nazwę obiektu    
            }

        }

    }

}    
