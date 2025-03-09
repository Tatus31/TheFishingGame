using System;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class StartFireWhenInCloud : MonoBehaviour
{
    public event Action OnShipInCloud;
    private void OnTriggerStay(Collider other)
    {    
        if(other.gameObject.CompareTag("Ship"))
        {
            OnShipInCloud?.Invoke();
            Debug.Log("Jest g");
        }

    }
	
    

}