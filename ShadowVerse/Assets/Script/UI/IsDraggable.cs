using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IsDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private TouchManager touchManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        touchManager.canMove = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        touchManager.canMove = true;
    }

}
