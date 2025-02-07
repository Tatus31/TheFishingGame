using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class FlippersState : MonoBehaviour
{
   public Player Player;
   public PlayerMovement PlayerMovement;
   public GameObject flippers;
   public Inventory inv;
   public Item it;
   public SwimmingState swim;
   public Transform snappingpoint;

    private void Start()
    {
        Debug.Log(flippers.transform.position);
    }
    private void Update()
    {

        float distance2 = Vector3.Distance(flippers.transform.position, snappingpoint.position);
        if (distance2 == 0)
        {
            PlayerMovement.SwimmingState.Move(PlayerMovement, 2, 10, 3);
            Debug.Log("Approved! stample noises in the background");
        }
        else
        {
            Debug.Log("Nie dziala");
        }



        Unequip();
      
        
      
      
   }


    public void Unequip()
    {
        float distance2 = Vector3.Distance(flippers.transform.position, snappingpoint.position);
        if (distance2 == 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                flippers.transform.position = new Vector3(1035.35f, 118.67f, 0.00f);
            }
        }
    }
}
