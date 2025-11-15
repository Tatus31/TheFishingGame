using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScript : MonoBehaviour
{
    [SerializeField] private RectTransform endRect;

    private void Start()
    {
        endRect.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endRect.gameObject.SetActive(true);
            StartCoroutine(Hide());
        }
    }

    private IEnumerator Hide()
    {
        if(!endRect.gameObject.activeSelf)
            yield break;
        
        yield return new WaitForSeconds(5f);
        endRect.gameObject.SetActive(false);
    }
}
