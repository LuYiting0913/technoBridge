using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        foreach (SolidBar bar in selectedBars) if (bar != null) bar.transform.GetChild(0).gameObject.SetActive(false);
        foreach (Point point in selectedPoints) if (point != null) point.transform.GetChild(0).gameObject.SetActive(false);
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
        int scanInterval = 5;
        int leftBound =  (int) Math.Round(Math.Min(firstCorner.x, SecondCorner.x));
        int rightBound = (int) Math.Round(Math.Max(firstCorner.x, SecondCorner.x));
        int lowerBound = (int) Math.Round(Math.Min(firstCorner.y, SecondCorner.y));
        int upperBound = (int) Math.Round(Math.Max(firstCorner.y, SecondCorner.y));

        for (int i = leftBound; i < rightBound; i += scanInterval) {
            for (int j = lowerBound; j < upperBound; j += scanInterval) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector3(i, j, -10), new Vector3(0, 0, 1));
                if (hit.collider != null) {
                    Debug.Log(hit.transform);
                    bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
                    if (!isActive) {
                        SelectionController.AddToSelection(hit.transform);
                    }
                    hit.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        firstCorner = Vector2.zero;
        SecondCorner = Vector2.zero;        
    }

    public static void ToggleIndividual(RaycastHit2D hit) {
        bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
        hit.transform.GetChild(0).gameObject.SetActive(!isActive);
        if (isActive) {
            // deselect
            SelectionController.RemoveFromSelection(hit.transform);
        } else {
            SelectionController.AddToSelection(hit.transform);
        }
    }

    private static void AddToSelection(Transform transform) {
        if (transform.GetComponent<SolidBar>() != null) {
            selectedBars.Add(transform.GetComponent<SolidBar>());
        } else {
            selectedPoints.Add(transform.GetComponent<Point>());
        }
    } 

    private static void RemoveFromSelection(Transform transform) {
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
