using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatControllerState : MonoBehaviour
{

    public GameObject player;
    public GameObject Boat;
    public GameObject BoatCamera;
    public GameObject PlayerStartpos;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1"))
        {
            Boat.GetComponent<Rigidbody>().isKinematic = false;
            Boat.GetComponent<BoatMove>().enabled = true;
            BoatCamera.SetActive(true);

            player.SetActive(false);
        }


        if (Input.GetKey("2"))
        {
            Boat.GetComponent<Rigidbody>().isKinematic = true;
            Boat.GetComponent<BoatMove>().enabled = false;
            BoatCamera.SetActive(false);

            player.SetActive(true);
            player.transform.position = PlayerStartpos.transform.position;
        }
    }

    
  

}
