using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionController : MonoBehaviour { 
    private static SelectionController m_Instance;

    private bool isActive = false;
    private bool boxSelection, draggingCopied= false;

    private List<SolidBar> selectedBars = new List<SolidBar>();
    private List<Point> selectedPoints = new List<Point>();
    private Vector2 firstCorner, SecondCorner;
    private Transform copiedParent;
    private Transform selectionBox;
    private GameObject dummyBar, dummyPoint;
    private List<Point> dummyPoints;
    private List<SolidBar> dummyBars;
    private Vector3 originalPosition;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            //DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public void Start() {
        firstCorner = Vector2.zero;
        SecondCorner = Vector2.zero;
        copiedParent = GameObject.Find("CopiedParent").transform; 
        GameObject template = Resources.Load<GameObject>("Prefab/SelectionBox");
        selectionBox = Instantiate(template, firstCorner, Quaternion.identity).
            GetComponent<Transform>();
    }

    public static SelectionController GetInstance() {
        return m_Instance;
    }
     
    public void OnModeChanged(object source, int i) {
        isActive = i == 1;
    }

    public void OnPressed(object source, Stage1Controller e) {
        if (isActive) {
            RaycastHit2D hit = Physics2D.Raycast(e.GetStartPoint(), new Vector3(0, 0, 1));
            if (draggingCopied) {
                originalPosition = copiedParent.transform.position;
            } else if (hit.collider != null && IsBarOrPoint(hit.transform)) {
                //individual select
                ToggleIndividual(hit);
            } else {
                // box select
                InitFirstCorner(e.GetStartPoint());
                boxSelection = true;
            }
        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive) {
            if (draggingCopied) {
                SnapToExistingPoint();
            } else if (boxSelection) {
                boxSelection = false;
                FinalizeBoxSelection();
            }
        }
    }
    
    public void OnDragged(object source, Stage1Controller e) {
        if (isActive) {
            if (draggingCopied) {
                Vector2 dir = e.GetCurPoint() - e.GetStartPoint();
                copiedParent.transform.position = originalPosition + new Vector3(dir.x, dir.y, 0);
            } else if (boxSelection) {
                InitSecondCorner(e.GetCurPoint());
                RenderSelectionBox();
            }
        }
    }

    public void ClearAll() {
        // copiedParent = GameObject.Find("CopiedParent").transform; 
        foreach (SolidBar bar in selectedBars) if (bar != null) bar.transform.GetChild(0).gameObject.SetActive(false);
        foreach (Point point in selectedPoints) if (point != null) point.transform.GetChild(0).gameObject.SetActive(false);
        selectedBars = new List<SolidBar>();
        selectedPoints = new List<Point>();
    }

    private bool IsBarOrPoint(Transform transform) {
        return transform.GetComponent<SolidBar>() != null || transform.GetComponent<Point>() != null;
    }

    public void ClearAllDummy() {
        foreach (Transform child in copiedParent) GameObject.Destroy(child.gameObject);
        dummyBars = new List<SolidBar>();
        dummyPoints = new List<Point>();
    }

    private void InitFirstCorner(Vector2 cursor) {
        firstCorner = cursor;
    }

    private void InitSecondCorner(Vector2 cursor) {
        SecondCorner = cursor;
    }

    private void RenderSelectionBox() {
        selectionBox.transform.position = (firstCorner + SecondCorner) / 2;
        selectionBox.transform.localScale = new Vector3(firstCorner.x - SecondCorner.x, firstCorner.y - SecondCorner.y, 0);
    }

    public void FinalizeBoxSelection() {
        selectionBox.transform.localScale = new Vector3(0, 0, 1);
        int scanInterval = 5;
        int leftBound =  (int) Math.Round(Math.Min(firstCorner.x, SecondCorner.x));
        int rightBound = (int) Math.Round(Math.Max(firstCorner.x, SecondCorner.x));
        int lowerBound = (int) Math.Round(Math.Min(firstCorner.y, SecondCorner.y));
        int upperBound = (int) Math.Round(Math.Max(firstCorner.y, SecondCorner.y));

        for (int i = leftBound; i < rightBound; i += scanInterval) {
            for (int j = lowerBound; j < upperBound; j += scanInterval) {
                RaycastHit2D hit = Physics2D.Raycast(new Vector3(i, j, -10), new Vector3(0, 0, 1));
                if (hit.collider != null && IsBarOrPoint(hit.transform)) {
                    bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
                    if (!isActive) {
                        AddToSelection(hit.transform);
                    }
                    hit.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        firstCorner = Vector2.zero;
        SecondCorner = Vector2.zero;        
    }

    private void ToggleIndividual(RaycastHit2D hit) {
        bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
        hit.transform.GetChild(0).gameObject.SetActive(!isActive);
        if (isActive) {
            // deselect
            RemoveFromSelection(hit.transform);
        } else {
            AddToSelection(hit.transform);
        }
    }

    private void AddToSelection(Transform transform) {
        //Debug.Log("selected");
        if (transform.GetComponent<SolidBar>() != null) {
            selectedBars.Add(transform.GetComponent<SolidBar>());
        } else {
            selectedPoints.Add(transform.GetComponent<Point>());
        }
    } 

    private void RemoveFromSelection(Transform transform) {
        if (transform.GetComponent<SolidBar>() != null) {
            selectedBars.Remove(transform.GetComponent<SolidBar>());
        } else {
            selectedPoints.Remove(transform.GetComponent<Point>());
        }
    }

    public void DeleteSelection() {
        //Debug.Log("deleted");
        foreach (SolidBar bar in selectedBars) {
            //Debug.Log(bar);
            DeleteBar(bar);
        }

        foreach (Point point in selectedPoints) {
            if (!point.IsFixed()) {
                // foreach (SolidBar bar in point.connectedBars) {
                //     DeleteBar(bar);
                // }
                DeletePoint(point);
            }
        }  
        AssetManager.RemoveConnectedNull();      
    }


    private void DeleteBar(SolidBar bar) {
        if (bar != null) {
            AssetManager.DeleteBar(bar);
            bar.head.DeleteConnectedBar(bar);
            bar.tail.DeleteConnectedBar(bar);
            
            // Destroy(bar.gameObject);
            bar.gameObject.SetActive(false);
        }
    }

    private void DeletePoint(Point point) {
        if (point != null) {
            AssetManager.DeletePoint(point);
            for (int i = 0; i < point.connectedBars.Count; i++) {
                AssetManager.DeleteBar(point.connectedBars[i]);
                if (point.Equals(point.connectedBars[i].head)) {
                    point.connectedBars[i].tail.DeleteConnectedBar(point.connectedBars[i]);
                } else {
                    point.connectedBars[i].head.DeleteConnectedBar(point.connectedBars[i]);
                }
                point.connectedBars[i].gameObject.SetActive(false);
                point.connectedBars[i] = null;
            }
            // Destroy(point.gameObject);
            point.gameObject.SetActive(false);
        }
    }

    public void CopySelected() {
        dummyPoint = Resources.Load<GameObject>("Prefab/DummyPoint");
        dummyBar = Resources.Load<GameObject>("Prefab/DummyBar");
        dummyPoints = new List<Point>();
        dummyBars = new List<SolidBar>();
        Vector3 offset = new Vector3(0, 50, 0);
        foreach (Point p in selectedPoints) {
            if (p != null) {
                Point newPoint = Instantiate(dummyPoint, p.GetPosition() + offset,
                                            Quaternion.identity, copiedParent).GetComponent<Point>();
                dummyPoints.Add(newPoint);
            }

        } 

        foreach (SolidBar b in selectedBars) {
            if (b != null) {
                SolidBar newBar = Instantiate(dummyBar, b.GetPosition() + offset,
                            b.transform.rotation, copiedParent).GetComponent<SolidBar>();
                Point newHead = Produce(b.head.GetPosition() + offset);
                Point newTail = Produce(b.tail.GetPosition() + offset);
                newBar.SetR(newHead, newTail);
                newBar.transform.localScale = new Vector3(newBar.GetLength(), 5);
                newBar.SetMaterial(b.GetMaterial());
                dummyBars.Add(newBar);
            }
        } 
        draggingCopied = true;
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
        draggingCopied = true;
    }

    public void Paste() {
        Transform barParent = GameObject.Find("BarParent").transform;
        Transform pointParent = GameObject.Find("PointParent").transform;
        GameObject pointTemplate = PrefabManager.GetPoint2DTemplate();
        List<SolidBar> newBars = new List<SolidBar>();
        List<Point> newPoints = new List<Point>();

        foreach (Point p in dummyPoints) {
            if (!AssetManager.HasPoint(p.GetWorldPosition())) {
                Point newPoint = Instantiate(pointTemplate, p.GetWorldPosition(), Quaternion.identity, pointParent).
                    GetComponent<Point>();
                newPoints.Add(newPoint);
                AssetManager.AddPoint(newPoint);     
            } else {
                Point newPoint = AssetManager.GetPoint(p.GetWorldPosition());
                newPoints.Add(newPoint);
            }
        }

        foreach (SolidBar bar in dummyBars) {
            GameObject barTemplate = MaterialManager.GetTemplate2D(bar.GetMaterial());
            SolidBar newBar = Instantiate(barTemplate, barParent).
                GetComponent<SolidBar>();
            // newBar.transform.localScale = new Vector2(bar.GetLength(), 10);
            Point newHead = Search(newPoints, bar.head.GetWorldPosition());
            Point newTail = Search(newPoints, bar.tail.GetWorldPosition());
            newBar.SetR(newHead, newTail);
            newHead.AddConnectedBar(newBar);
            newTail.AddConnectedBar(newBar);
            newBar.SetMaterial(bar.GetMaterial());
            newBar.RenderSolidBar(1);
            AssetManager.AddBar(newBar);
        }
        draggingCopied = false;
    }

    // snap upon release, instead of per frame, to reduce amount of calculation
    public void SnapToExistingPoint() {
        foreach (Point p in dummyPoints) {
            Vector3 pos = p.GetWorldPosition();
            if (AssetManager.HasSnap(pos)) {
                Vector3 targetPos = AssetManager.GetSnap(pos).GetWorldPosition();
                Vector3 dir = targetPos - pos;
                copiedParent.transform.position += dir;
            }
        }

    }

    public void FlipHorizontally() {
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

    public void FlipVertically() {
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

    private Point Produce(Vector3 pos) {
        foreach (Point p in dummyPoints) {
            if (p.Contain(pos)) {
                return p;
            }
        }
        Point point = Instantiate(dummyPoint, pos, Quaternion.identity, copiedParent).GetComponent<Point>();
        dummyPoints.Add(point);
        return point;
    }

    private Point Search(List<Point> points, Vector3 pos) {
        foreach (Point p in points) {
            if (p.Contain(pos)) return p;
        }
        return null;
    }

}
