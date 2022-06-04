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
    private static Transform copiedParent;
    private static GameObject dummyBar, dummyPoint;
    private static List<Point> dummyPoints;
    private static List<SolidBar> dummyBars;
    // private static List<Point> pastedPoints;
    // private static List<SolidBar> pastedBars;
    //public static Transform barParent;

    public void Start() {
        firstCorner = Vector2.zero;
        SecondCorner = Vector2.zero;
        copiedParent = GameObject.Find("CopiedParent").transform; 
        GameObject template = Resources.Load<GameObject>("Prefab/SelectionBox");
        SelectionBox = Instantiate(template, firstCorner, Quaternion.identity).
            GetComponent<Transform>();
    }

    public static void ClearAll() {
        // copiedParent = GameObject.Find("CopiedParent").transform; 
        foreach (SolidBar bar in selectedBars) if (bar != null) bar.transform.GetChild(0).gameObject.SetActive(false);
        foreach (Point point in selectedPoints) if (point != null) point.transform.GetChild(0).gameObject.SetActive(false);
        selectedBars = new List<SolidBar>();
        selectedPoints = new List<Point>();
    }

    public static void ClearAllDummy() {
        foreach (Transform child in copiedParent) GameObject.Destroy(child.gameObject);
        dummyBars = new List<SolidBar>();
        dummyPoints = new List<Point>();
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

    public static bool SthSelected() {
        return selectedBars.Count > 0 || selectedPoints.Count > 0;
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

    public void CopySelected() {
        dummyPoint = Resources.Load<GameObject>("Prefab/DummyPoint");
        dummyBar = Resources.Load<GameObject>("Prefab/DummyBar");
        dummyPoints = new List<Point>();
        dummyBars = new List<SolidBar>();
        Vector3 offset = new Vector3(0, 50, 0);
        foreach (Point p in selectedPoints) {
            Point newPoint = Instantiate(dummyPoint, p.GetPosition() + offset,
                        Quaternion.identity, copiedParent).GetComponent<Point>();
            // newPoint.SetFree();
            dummyPoints.Add(newPoint);
        } 

        foreach (SolidBar b in selectedBars) {
            SolidBar newBar = Instantiate(dummyBar, b.GetPosition() + offset,
                        b.transform.rotation, copiedParent).GetComponent<SolidBar>();
            Point newHead = Produce(b.head.GetPosition() + offset);
            Point newTail = Produce(b.tail.GetPosition() + offset);
            newBar.SetR(newHead, newTail);
            newBar.transform.localScale = new Vector3(newBar.GetLength(), 5);
            newBar.SetMaterial(b.GetMaterial());
            dummyBars.Add(newBar);
        } 
        //ClearAll();
    }

    public void CutSelected() {
        CopySelected();
        foreach (SolidBar b in selectedBars) {
            AssetManager.DeleteBar(b);
            b.head.DeleteConnectedBar(b);
            b.tail.DeleteConnectedBar(b);
            GameObject.Destroy(b.gameObject);
        }

        foreach (Point p in AssetManager.GetAllPoints()) {
            if (p.IsSingle()) {
                AssetManager.DeletePoint(p);
                GameObject.Destroy(p.gameObject);
            }
        }
    }

    public static void Paste() {
        Transform barParent = GameObject.Find("BarParent").transform;
        Transform pointParent = GameObject.Find("PointParent").transform;
        GameObject pointTemplate = PrefabManager.GetPoint2DTemplate();
        List<SolidBar> newBars = new List<SolidBar>();
        List<Point> newPoints = new List<Point>();

        foreach (Point p in dummyPoints) {
            Point newPoint = Instantiate(pointTemplate, p.GetPosition(), Quaternion.identity, pointParent).
                GetComponent<Point>();
            newPoints.Add(newPoint);
            AssetManager.AddPoint(newPoint);     
        }

        foreach (SolidBar bar in dummyBars) {
            GameObject barTemplate = MaterialManager.GetTemplate2D(bar.GetMaterial());
            SolidBar newBar = Instantiate(barTemplate, barParent).
                GetComponent<SolidBar>();
            // newBar.transform.localScale = new Vector2(bar.GetLength(), 10);
            Point newHead = Search(newPoints, bar.GetHead());
            Point newTail = Search(newPoints, bar.GetTail());
            newBar.SetR(newHead, newTail);
            newHead.AddConnectedBar(newBar);
            newTail.AddConnectedBar(newBar);
            newBar.SetMaterial(bar.GetMaterial());
            newBar.RenderSolidBar(1);
            AssetManager.AddBar(newBar);
        }
    }

    // snap upon release, instead of per frame, to reduce amount of calculation
    public static void SnapToExistingPoint() {
        foreach (Point p in dummyPoints) {
            Vector3 pos = p.GetPosition();
            if (AssetManager.HasSnap(pos)) {
                Vector3 targetPos = AssetManager.GetSnap(pos).GetPosition();
                Vector3 dir = targetPos - pos;
                copiedParent.transform.position += dir;
            }
        }

    }

    public static void FlipHorizontally() {
        float x = copiedParent.transform.position.y;
        foreach (Point p in dummyPoints) {
            Vector3 pos = p.transform.position;
            p.transform.position = new Vector3(2 * x - pos.x, pos.y, pos.z);
        } 

        foreach (SolidBar b in dummyBars) {
            Vector3 pos = b.transform.position;
            b.transform.position = new Vector3(2 * x - pos.x, pos.y, pos.z);
            b.transform.rotation = Quaternion.Euler(0, 0, - b.transform.eulerAngles.z);
        } 
    }

    public static void FlipVertically() {
        float y = copiedParent.transform.position.y;
        foreach (Point p in dummyPoints) {
            Vector3 pos = p.transform.position;
            p.transform.position = new Vector3(pos.x, 2 * y - pos.y, pos.z);
        } 

        foreach (SolidBar b in dummyBars) {
            Vector3 pos = b.transform.position;
            b.transform.position = new Vector3(pos.x, 2 * y - pos.y, pos.z);
            b.transform.rotation = Quaternion.Euler(0, 0, - b.transform.eulerAngles.z);
        } 
    }

    private static Point Produce(Vector3 pos) {
        foreach (Point p in dummyPoints) {
            if (p.Contain(pos)) {
                return p;
            }
        }
        Point point = Instantiate(dummyPoint, pos, Quaternion.identity, copiedParent).GetComponent<Point>();
        dummyPoints.Add(point);
        return point;
    }

    private static Point Search(List<Point> points, Vector3 pos) {
        foreach (Point p in points) {
            if (p.Contain(pos)) return p;
        }
        return null;
    }

}
