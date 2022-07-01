using System.Collections;
using System.Collections.Generic;
// using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class TraceController : MonoBehaviour {
    private static TraceController m_Instance; 
    private bool isActive = false;
    private float scale;

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
        if (isActive) {
            // Debug.Log("stracecontroller receieved press");
            e.ActivateCursor();
            StartTrace(e.GetStartPoint(), e.GetCurrentMaterial(), e.barParent, e.pointParent);
            scale = Stage1Controller.backgroundScale;
        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("tracecontroller receieved release");
            e.DeactivateCursor();
            EndTrace();
        }
    }

    public void OnDragged(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("tracecontroller receieved drag");
            e.UpdateCursor(e.GetCurPoint());
            RenderDummy(e.GetCurPoint());
        }
    }

    public void Update() {

    }

    public void StartTrace(Vector2 headPosition, int m, Transform bParent, Transform pParent) {
        DestroyAllDummy();
        // if (dummyBar != null) GameObject.Destroy(dummyBar.gameObject);
        // dummyBar = null;
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

        float maxLength = MaterialManager.GetMaxLength(material);
        Vector2 headPos = head.transform.position;
        Vector2 tailPos = tail.transform.position;
        Vector2 dir = tailPos - headPos;
        int numberOfSegments = (int) Math.Ceiling(dir.magnitude / maxLength);
        for (int i = 1; i < numberOfSegments; i++) {
            Vector2 position = headPos + dir * i / numberOfSegments;
            Point dummy = Instantiate(dummyPointTemplate, position, Quaternion.identity, traceParent).GetComponent<Point>();
            guidePoints.Add(dummy);
        }
        GameObject traceToolBar = GameObject.Find("PopupToolBar").transform.GetChild(2).gameObject;
        traceToolBar.SetActive(true);
    }

    public void FillTrace() {
        Point currHead = head;
        Point currTail;
        GameObject barTemplate = MaterialManager.GetTemplate2D(material);
        for (int i = 0; i < guidePoints.Count; i++) {
            currTail = Instantiate(PointTemplate, guidePoints[i].GetPosition(), Quaternion.identity, pointParent).GetComponent<Point>();
            SolidBar b = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
            b.SetR(currHead, currTail);
            currHead.AddConnectedBar(b);
            currTail.AddConnectedBar(b);
            // b.RenderSolidBar(Stage1Controller.backgroundScale);
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
    }
}
