using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionDetectionFire : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Fire")) // Upewnij się, że Twój Cube ma przypisany tag "Cube"
        {   
            //other.SetActive(false);
            Destroy(other);
            Debug.Log("Zniszony");
        }
    }
}
