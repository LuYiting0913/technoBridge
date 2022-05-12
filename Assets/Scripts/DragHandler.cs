using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler {

    //[SerializeField] private Canvas canvas;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("onPointerDown");
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("onBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("onEndDrag");
    }

    public void OnDrag(PointerEventData eventData) {
        Debug.Log("onDrag");
        rectTransform.anchoredPosition += eventData.delta;/// canvas.scaleFactor;
    }
}
