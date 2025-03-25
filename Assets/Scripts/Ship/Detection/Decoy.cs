using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Decoy : MonoBehaviour
{
    public static event EventHandler<Transform> OnDecoyActivated;

    [SerializeField] GameObject decoyObj;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(decoyObj == null)
            {
#if UNITY_EDITOR
                Debug.Log("there is no decoyObj");
#endif
                return;
            }
            Instantiate(decoyObj, transform.position, Quaternion.identity);
            decoyObj.transform.position = transform.position;
            OnDecoyActivated?.Invoke(this, decoyObj.transform);
        }
    }
}
