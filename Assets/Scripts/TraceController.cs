using System.Collections;
using System.Collections.Generic;
// using UnityEngine.EventSystems;
using UnityEngine;
using UnityEditor;
using System;

public class TraceController : MonoBehaviour {
    private static TraceController m_Instance; 
    private bool isActive = false;
    private bool isEditing = false;
    private float scale;
    private int lineType = 0;
    // 0: straight, 1: curve

    private int material;
    private Point head, tail;
    private SolidBar dummyBar;
    private List<Point> guidePoints = new List<Point>();

    public Transform traceParent, barParent, pointParent;
    public GameObject dummyPointTemplate;
    public GameObject PointTemplate;
    public GameObject dummyBarTemplate;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            //DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public static TraceController GetInstance() {
        return m_Instance;
    }

    public void OnModeChanged(object source, int i) {
        isActive = i == 4;
    }

    public void OnPressed(object source, Stage1Controller e) {
        if (isActive && !isEditing) {
            // Debug.Log("stracecontroller receieved press");
            e.ActivateCursor();
            StartTrace(e.GetStartPoint(), e.GetCurrentMaterial(), e.barParent, e.pointParent);
            scale = Stage1Controller.backgroundScale;

        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive && !isEditing) {
            // Debug.Log("tracecontroller receieved release");
            e.DeactivateCursor();
            EndTrace();
        }
    }

    public void OnDragged(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("tracecontroller receieved drag");
            if (!isEditing) {
                e.UpdateCursor(e.GetCurPoint());
                RenderDummy(e.GetCurPoint());
            } else {
                DestroyAllDummy();
                InstantiateGuidePoint(GetComponent<CurveEditor>().UpdateEditor(e.GetCurPoint()));
            }
        }
    }

    public void SetStraightLine() {
        lineType = 0;
        InstantiateGuidePoint(GetComponent<CurveEditor>().DrawTrace(head.transform.position, tail.transform.position, material, lineType));
    }

    public void SetCurve() {
        lineType = 1;
        InstantiateGuidePoint(GetComponent<CurveEditor>().DrawTrace(head.transform.position, tail.transform.position, material, lineType));
    }


    public void StartTrace(Vector2 headPosition, int m, Transform bParent, Transform pParent) {
        DestroyAllDummy();

        material = m;
        barParent = bParent;
        pointParent = pParent;
        guidePoints = new List<Point>();
        dummyBar = Instantiate(dummyBarTemplate, traceParent).GetComponent<SolidBar>();
        if (AssetManager.HasPoint(headPosition)) {
            head = AssetManager.GetPoint(headPosition);
        } else {
            head = Instantiate(PointTemplate, headPosition, Quaternion.identity, traceParent).GetComponent<Point>();
            AssetManager.AddPoint(head);
        }
        tail = Instantiate(PointTemplate, headPosition, Quaternion.identity, traceParent).GetComponent<Point>();
        dummyBar.SetR(head, tail);
    }

    public void EndTrace() {
        if (AssetManager.HasPoint(tail.transform.position)) {
            GameObject.Destroy(tail.gameObject);
            tail = AssetManager.GetPoint(tail.transform.position);
        } else {
            AssetManager.AddPoint(tail);
        }
        InstantiateGuidePoint(GetComponent<CurveEditor>().DrawTrace(head.transform.position, tail.transform.position, material, lineType));

        GameObject traceToolBar = GameObject.Find("PopupToolBar").transform.GetChild(2).gameObject;
        traceToolBar.SetActive(true);
        isEditing = true;
    }

    private void InstantiateGuidePoint(List<Vector3> points) {
        DestroyAllDummy();
        foreach (Vector3 v in points) {
            Point dummy = Instantiate(dummyPointTemplate, v, Quaternion.identity, traceParent).GetComponent<Point>();
            guidePoints.Add(dummy);
        }
    }

    public void FillTrace() {
        isEditing = false;
        GetComponent<CurveEditor>().Close();
        Point currHead = head;
        Point currTail;
        GameObject barTemplate = MaterialManager.GetTemplate2D(material);
        for (int i = 1; i < guidePoints.Count - 1; i++) {
            currTail = Instantiate(PointTemplate, guidePoints[i].transform.position, Quaternion.identity, pointParent).GetComponent<Point>();
            SolidBar b = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
            b.SetR(currHead, currTail);
            currHead.AddConnectedBar(b);
            currTail.AddConnectedBar(b);
            AssetManager.AddPoint(currTail);
            AssetManager.AddBar(b);
            
            currHead = currTail;
        }
        SolidBar bar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        bar.SetR(currHead, tail);
        currHead.AddConnectedBar(bar);
        tail.AddConnectedBar(bar);
        // bar.RenderSolidBar(Stage1Controller.backgroundScale);
        AssetManager.AddBar(bar);
        
    }

    public void ToggleLineType() {

    }

    public void RenderDummy(Vector2 cursor) {
        tail.transform.position = cursor;
        dummyBar.RenderSolidBar(scale);
    }

    public void DestroyAllDummy() {
        foreach (Point p in guidePoints) if (p != null) GameObject.Destroy(p.gameObject);
        if (dummyBar != null) GameObject.Destroy(dummyBar.gameObject);
        guidePoints = new List<Point>();
    }

    public void Update() {
        // if (lineType == 1) {
            
        // }
    }
}
