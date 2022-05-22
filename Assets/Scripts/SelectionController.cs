using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour { 
    private static List<SolidBar> selectedBars = new List<SolidBar>();
    private static List<Point> selectedPoints = new List<Point>();

    public static void ClearAll() {
        selectedBars = new List<SolidBar>();
        selectedPoints = new List<Point>();
    }

    public static void AddToSelection(Transform transform) {
        if (transform.GetComponent<SolidBar>() != null) {
            selectedBars.Add(transform.GetComponent<SolidBar>());
        } else {
            selectedPoints.Add(transform.GetComponent<Point>());
        }
    } 

    public static void RemoveFromSelection(Transform transform) {
        if (transform.GetComponent<SolidBar>() != null) {
            selectedBars.Remove(transform.GetComponent<SolidBar>());
        } else {
            selectedPoints.Remove(transform.GetComponent<Point>());
        }
    }

    public static void DeleteSelection() {
        foreach (SolidBar bar in selectedBars) {
            if (bar != null) {
                AssetManager.DeleteBar(bar);
                Destroy(bar.gameObject);
            }
        }

        foreach (Point point in selectedPoints) {
            if (point != null && !point.IsFixed()) {
                AssetManager.DeletePoint(point);
                Destroy(point.gameObject);
            }
        }        
    }

}
