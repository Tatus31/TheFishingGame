using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;

public class FlippersState : MonoBehaviour
{
   public Player Player;
   public PlayerMovement PlayerMovement;
   public GameObject flippers;
   public Inventory inv;
   public Item it;
   public SwimmingState swim;

   
   private void Update()
   { 
      Transform childTransform = transform.Find("Flippers");
      if (childTransform != null) 
      {
         PlayerMovement.SwimmingState.Move(PlayerMovement,2,10,3);
         Debug.Log("Approved! stample noises in the background");
         
      } 
      else 
      {
         Debug.Log("Jest Kaka");
      }
      
   }
}
