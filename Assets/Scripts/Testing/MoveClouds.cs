using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using Random = UnityEngine.Random;

public class MoveClouds : MonoBehaviour
{
    [SerializeField] private Transform Cloud;
    
    
    
    private void Update()
    {
        MoveCloud(0.10f);
    }
    
    void MoveCloud(float moveamount)
    {
        if (Random.value == 5f)
        {
            Cloud.Translate(moveamount * Time.deltaTime,0,0);    
        }
        else{Cloud.Translate(-(moveamount * Time.deltaTime),0,0);}
        
    }

    
}
