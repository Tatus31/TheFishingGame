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
    private CanvasGroup m_CanvasGroup;
    [SerializeField]private Canvas canvas;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_CanvasGroup.blocksRaycasts = false;
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
        m_CanvasGroup.blocksRaycasts = true;
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
