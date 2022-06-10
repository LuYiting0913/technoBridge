using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour {
    private static DragController m_Instance;
    private bool isActive, isDragging, isDraggingBackground;
    
    private Transform selectedPoint;
    private Vector2 initialPosition;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public static DragController GetInstance() {
        return m_Instance;
    }

    public void OnModeChanged(object source, int i) {
        isActive = i == 2;
    }

    public void OnPressed(object source, Stage1Controller e) {
        if (isActive) {
            RaycastHit2D hit = Physics2D.Raycast(e.startPoint, new Vector3(0, 0, 1));
            if (hit.collider != null) {
                e.ActivateCursor();
                Point hittedPoint = hit.transform.GetComponent<Point>();
                if (hittedPoint != null && !hittedPoint.IsFixed()) {
                    SelectPoint(hit.transform);
                    isDragging = true;
                }
            } else {
                // drag background
                isDraggingBackground = true;
            }
        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive) {
            if (isDragging) {
                e.DeactivateCursor();
                ReleasePoint();
            } else if (isDraggingBackground) {
                e.backgroundPosition = e.gameObject.transform.position; 
            }
            isDragging = false;
            isDraggingBackground = false;
        }
    }

    public void OnDragged(object source, Stage1Controller e) {
        if (isActive) {
            if (isDragging) {
            e.UpdateCursor(DragPointTo(e.curPoint, Stage1Controller.backgroundScale));
            } else if (isDraggingBackground) {
                Vector2 dir = e.curPoint - e.startPoint;
                e.gameObject.transform.position = e.backgroundPosition + new Vector3(dir.x, dir.y, 0);
            }
        }
    }

    private void SelectPoint(Transform point) {
        selectedPoint = point;
        initialPosition = point.transform.position;
        selectedPoint.GetChild(0).gameObject.SetActive(true);
    }

    private void ReleasePoint() {
        selectedPoint.GetChild(0).gameObject.SetActive(false);
        selectedPoint = null;
    }

    private Vector2 DragPointTo(Vector2 cursor, float scale) {
        Point point = selectedPoint.GetComponent<Point>();
        point.transform.position = point.GetReachablePosition(point.transform.position, cursor, scale);
        point.UpdateConnectedBars();
        return new Vector2(point.transform.position.x, point.transform.position.y);
    }
}
