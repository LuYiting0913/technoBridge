using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
    public Camera myCam;
    private Vector3 originalPosition;
    private bool isDragging;
    private Vector2 start;
    
    
    public void OnPointerDown(PointerEventData eventData) {
        // Debug.Log(Camera.main.ScreenToWorldPoint(eventData.position));
        isDragging = true;
        Vector3 temp = Input.mousePosition;
        start = new Vector2(temp.x, temp.y);
        originalPosition = myCam.transform.position;
    }

    public void OnPointerUp(PointerEventData eventData) {
        // Debug.Log(Camera.main.ScreenToWorldPoint(eventData.position));
        isDragging = false;
    }

    public void Update() {
        if (isDragging) {
            Vector2 cursor = Input.mousePosition;
            Debug.Log(cursor);
            float ratio = myCam.orthographicSize / 100;
            Vector2 dir = cursor - start;
            myCam.transform.position = originalPosition - new Vector3(dir.x, dir.y, 0) / 2.4f;
        }
    }

}
