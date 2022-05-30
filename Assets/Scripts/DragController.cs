using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour {
    private static Transform selectedPoint;
    private static Vector2 initialPosition;

    public static void SelectPoint(Transform point) {
        selectedPoint = point;
        initialPosition = point.transform.position;
        selectedPoint.GetChild(0).gameObject.SetActive(true);
    }

    public static void ReleasePoint() {
        selectedPoint.GetChild(0).gameObject.SetActive(false);
        selectedPoint = null;
    }

    public static void DragPointTo(Vector2 cursor) {
        Point point = selectedPoint.GetComponent<Point>();
        point.transform.position = point.GetReachablePosition(point.transform.position, cursor);
        point.UpdateConnectedBars();
    }
}
