using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class ItemSelector : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    RectTransform m_RectTransform;
    [SerializeField]private Canvas canvas;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //GetComponent<CanvasGroup>().blocksRaycasts = true;
        //Debug.Log("Starting dragging");
    }

    public void OnDrag(PointerEventData eventData)
    {

        m_RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //if (eventData.pointerCurrentRaycast.isValid)
        //{
        //    var currentRaycastPosition = eventData.pointerCurrentRaycast.worldPosition;
        //    transform.position = currentRaycastPosition;
        //    Debug.Log(currentRaycastPosition);
        //}

        //Debug.Log("Draging to drag");
       
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
        //Debug.Log("Ending dragging");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name + "was clicked");
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("Is Dropped");
    }
}
