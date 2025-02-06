using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapObj : MonoBehaviour
{
    public Transform snappingpoint;
    public Transform itemtosnap;
    public GameObject item2;
    private FireExtinguisheractivation fireExtinguisheractivation;
    //public Player player;

    private void Start()
    {
        
    }

    private void Update()
    {
        Snap(itemtosnap);
        //CheckIfItemInSlot();
    }

    public void Snap(Transform item)
    {
        float distance = Vector3.Distance(item.position, snappingpoint.position);

        if (distance < 10)
        {
            item2.gameObject.transform.position = snappingpoint.position;
        }

        //Debug.Log(distance);
    }

    //public void CheckIfItemInSlot()
    //{
    //    if (item2 != null)
    //    {
    //        if (item2 is GameObject)
    //        {
    //            float distance2 = Vector3.Distance(item2.transform.position, snappingpoint.position);
    //            Debug.Log(distance2);
    //            if (distance2 == 0)
    //            {   
                   
    //                Debug.LogError("Dziala");
    //            }
    //            else
    //            {
    //                Debug.Log("Nie dziala");
    //            }
    //        }
    //    }
    //}
}
