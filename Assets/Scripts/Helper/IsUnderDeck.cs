using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsUnderDeck : MonoBehaviour
{
    public static bool isUnderDeck = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isUnderDeck = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            isUnderDeck = false;
    }
}
