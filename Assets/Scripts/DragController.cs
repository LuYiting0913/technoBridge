using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour {
    private static Transform selectedPoint;

    public static void SelectPoint(Transform point) {
        selectedPoint = point;
        selectedPoint.GetChild(0).gameObject.SetActive(true);
    }

    public static void ReleasePoint() {
        selectedPoint.GetChild(0).gameObject.SetActive(false);
        selectedPoint = null;
    }

    public static void DragPointTo(Vector2 cursor) {
        Point point = selectedPoint.GetComponent<Point>();
        if (!point.ExceedsMaxLength(cursor)) {
            Debug.Log("dont exceed");
            point.transform.position = cursor;
            //point.UpdatePosition();
            point.UpdateConnectedBars();
        }

    }
}
