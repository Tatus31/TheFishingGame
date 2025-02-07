using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.Progress;

public class ToolBox : MonoBehaviour
{
    [SerializeField]
    private GameObject cube;

    //[SerializeField]
    //private Texture texture;
    //[SerializeField]
    //private Texture texture2;

    private Renderer renderer;

    [SerializeField]
    private Camera FPCamera;

    [SerializeField]
    private Texture[] textures;

    private int textureindex = 0;

    public Transform snappingpoint;
    public GameObject item2;

    private void Update()
    {   

        IsHeLooking();
        Unequip();
    }

   

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        cube = GetComponent<GameObject>();
        Debug.Log(item2.transform.position);
 
    }


    public void Unequip()
    {
        float distance2 = Vector3.Distance(item2.transform.position, snappingpoint.position);
        if (distance2 == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
               item2.transform.position = new Vector3(1106.37f,118.67f,0.00f);
            }
        }
    }

    private void ChangeCubeTexture()
    {

        float distance2 = Vector3.Distance(item2.transform.position, snappingpoint.position);

        if (distance2 == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log("Najpierw to");
                textureindex = textureindex + 1;
                renderer.material.mainTexture = textures[textureindex];

               
            }
        }
        else
        {
            Debug.Log("DUpa");
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
                ChangeCubeTexture();
                Debug.Log("Patrzysz na obiekt: " + hit.collider.gameObject.name); // Wypisuje nazwę obiektu    
            }

        }

    }

}    
