using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour { 
    private static List<SolidBar> selectedBars = new List<SolidBar>();
    private static List<Point> selectedPoints = new List<Point>();
    private static Vector2 firstCorner;
    private static Vector2 SecondCorner;
    private static Transform SelectionBox;
    //public static Transform barParent;

    public void Start() {
        firstCorner = Vector2.zero;
        SecondCorner = Vector2.zero;
        GameObject template = Resources.Load<GameObject>("Prefab/SelectionBox");
        SelectionBox = Instantiate(template, firstCorner, Quaternion.identity).
            GetComponent<Transform>();
    }

    public static void ClearAll() {
        selectedBars = new List<SolidBar>();
        selectedPoints = new List<Point>();
    }

    public static void InitFirstCorner(Vector2 cursor) {
        firstCorner = cursor;
    }

    public static void InitSecondCorner(Vector2 cursor) {
        SecondCorner = cursor;
    }

    public static void RenderSelectionBox() {
        SelectionBox.transform.position = (firstCorner + SecondCorner) / 2;
        SelectionBox.transform.localScale = new Vector3(firstCorner.x - SecondCorner.x, firstCorner.y - SecondCorner.y, 0);
    }

    public static void FinalizeBoxSelection() {
        SelectionBox.transform.localScale = new Vector3(0, 0, 1);
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
            DeleteBar(bar);
        }

        foreach (Point point in selectedPoints) {
            if (!point.IsFixed()) {
                foreach (SolidBar bar in point.connectedBars) {
                    DeleteBar(bar);
                }
                DeletePoint(point);
            }
        }        
    }

    private static void DeleteBar(SolidBar bar) {
        if (bar != null) {
            AssetManager.DeleteBar(bar);
            Destroy(bar.gameObject);
        }
    }

    private static void DeletePoint(Point point) {
        if (point != null) {
            AssetManager.DeletePoint(point);
            Destroy(point.gameObject);
        }
    }



}
