using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VisualScripting;
using static UnityEditor.Progress;

public class FlippersState : MovementBaseState
{
    public PlayerMovement player;
    float maxSpeed;
    float accelAmount;
    //public GameObject flippers;
    //public Inventory inv;
    //public Item it;
    //public SwimmingState swim;
    //public Transform snappingpoint;

    public FlippersState(PlayerMovement player, float maxSpeed, float accelAmount)
    {
        this.player = player;
        this.maxSpeed = maxSpeed;
        this.accelAmount = accelAmount;
    }

    // private void Update()
    // {

    //float distance2 = Vector3.Distance(flippers.transform.position, snappingpoint.position);
    //if (distance2 == 0)
    //{
    //    PlayerMovement.SwimmingState.Move(PlayerMovement, 2, 10, 3);
    //    Debug.Log("Approved! stample noises in the background");
    //}
    //else
    //{
    //    Debug.Log("Nie dziala");
    //}



    //Unequip();




    //}


    //public void Unequip()
    //{
    //    float distance2 = Vector3.Distance(flippers.transform.position, snappingpoint.position);
    //    if (distance2 == 0)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Mouse1))
    //        {
    //            flippers.transform.position = new Vector3(1035.35f, 118.67f, 0.00f);
    //        }
    //    }
    //}

    public override void EnterState(PlayerMovement player)
    {
        Debug.Log("flipper state");
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {
        base.Move(player, maxSpeed, maxSpeed, accelAmount);
        ApplyFriction(player, 1);
    }
    public override void ApplyFriction(PlayerMovement player, float frictionAmount)
    {
        base.ApplyFriction(player, frictionAmount);
    }

    public override void PlayAnimation(PlayerMovement player)
    {

    }
}
