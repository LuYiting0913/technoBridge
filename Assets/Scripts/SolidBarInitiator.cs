using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SolidBarInitiator : MonoBehaviour {
    private static SolidBarInitiator m_Instance;
    private bool isActive;

    private bool startedInit = false;
    public SolidBar currentBar;
    public Point beginPoint;
    public Point endPoint;
    public GameObject boundaryPoint;
    private int currentMaterial = 1;
    // 0: add bar, 1: select bar
    private int threshold = 5;

    // public int level;
    private GameObject barTemplate;
    private Transform barParent;
    private GameObject pointTemplate;
    private GameObject fixedPointTemplate;
    private Transform pointParent;
    private GameObject boundary;
    // public LayerMask clickable;

    private void Awake() {
        if (m_Instance == null) {
            m_Instance = this;
            //DontDestroyOnLoad(m_Instance);
        } else if (m_Instance != this) {
            Destroy(m_Instance);
        }
    }

    public static SolidBarInitiator GetInstance() {
        return m_Instance;
    }

    public void Start() {
        pointTemplate = PrefabManager.GetPoint2DTemplate();
        fixedPointTemplate = PrefabManager.GetFixedPoint2DTemplate();
        boundary = GameObject.Find("Boundary");
    }

    public void OnModeChanged(object source, int i) {
        isActive = i == 0;
    }

    public void OnPressed(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("add receieved press");
            currentMaterial = e.currentMaterial;
            e.ActivateCursor();
            ActivateBoundary(e.startPoint);
            InitializeBar(e.startPoint, e.currentMaterial, e.pointParent, e.barParent);
        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("add receieved release");
            e.DeactivateCursor();
            DeactivateBoundary();
            FinalizeBar(e.endPoint, e.autoTriangulate, Stage1Controller.backgroundScale);
        }
    }

    public void OnDragged(object source, Stage1Controller e) {
        if (isActive) {
            // Debug.Log("add receieved drag");
            float scale = Stage1Controller.backgroundScale;
            Vector2 cutOffVector = currentBar.CutOff(e.curPoint, scale);
            e.UpdateCursor(cutOffVector);

            if (e.autoComplete && endPoint.ExceedsMaxLength(e.curPoint, scale)) {
                FinalizeBar(cutOffVector, e.autoTriangulate, scale);
                InitializeBar(cutOffVector, e.currentMaterial, e.pointParent, e.barParent);
            } else { 
                endPoint.transform.position = cutOffVector;
                currentBar.RenderSolidBar(scale);
            }
        }
    }

    private  void ClearAll() {
        currentBar = null;
        beginPoint = null;
        endPoint = null;
    }

    public void InitializeBar(Vector2 headPos, int material, Transform pParent, Transform bParent) {
        ClearAll();

        startedInit = true;
        currentMaterial = material;
        barParent = bParent;
        pointParent = pParent;
        barTemplate = MaterialManager.GetTemplate2D(currentMaterial);
        // pointTemplate = Resources.Load<GameObject>("Prefab/Point");

        currentBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        Vector3 head = new Vector3(headPos.x, headPos.y, 0);
        
        // check if beginPoint already exists
        if (AssetManager.HasPoint(headPos)) {
            beginPoint = AssetManager.GetPoint(headPos);
            currentBar.SetHead(beginPoint);
        } else {
            beginPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
            AssetManager.AddPoint(beginPoint);
            currentBar.SetHead(beginPoint);
        }
        endPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
        currentBar.SetTail(endPoint);    
        beginPoint.AddConnectedBar(currentBar);
        endPoint.AddConnectedBar(currentBar);
    }

    public void FinalizeBar(Vector2 tailPos, bool autoComplete, float scale) {
        startedInit = false;
        Vector3 cutOffVector = currentBar.CutOff(new Vector3(tailPos.x, tailPos.y, 0), scale);

        // check if endPoint already exists
        if (AssetManager.HasPoint(cutOffVector)) {
            Destroy(endPoint.gameObject);
            endPoint = AssetManager.GetPoint(cutOffVector);
        } else {
            endPoint.transform.position = cutOffVector; 
            // endPoint.UpdatePosition();
            AssetManager.AddPoint(endPoint);
        }
        
        if (autoComplete) AutoTriangulate();

        currentBar.SetR(beginPoint, endPoint);
        AssetManager.AddBar(currentBar);
    }

    private  void AutoTriangulate() {
        List<Point> allPoints = AssetManager.GetAllPoints();
        foreach (Point p in allPoints) {
            if (p.DistanceTo(endPoint) <= MaterialManager.GetMaxLength(currentMaterial) &&
                    !p.Contain(endPoint.GetPosition()) && !p.Contain(beginPoint.GetPosition()) &&
                    !AssetManager.HasBar(endPoint, p)) {
                // can connect to this point
                SolidBar additionalBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
                additionalBar.SetHead(endPoint);
                additionalBar.SetTail(p);
                endPoint.AddConnectedBar(additionalBar);
                p.AddConnectedBar(additionalBar);
                AssetManager.AddBar(additionalBar);
            }
        }
        AssetManager.UpdatePoints(allPoints);
    }

    private void DeleteBar() {
        startedInit = false;
        Destroy(currentBar.gameObject);
        //AssetManager.DeleteBar(currentBar);
        if (beginPoint.IsSingle() && !beginPoint.IsFixed()) {
            Destroy(beginPoint.gameObject);
            AssetManager.DeletePoint(beginPoint);
        }
        if (endPoint.IsSingle() && !endPoint.IsFixed()) {
            Destroy(endPoint.gameObject);
            AssetManager.DeletePoint(endPoint);
        }
    }

    private void ActivateBoundary(Vector2 center) {
        for (int i = 0; i < boundary.transform.childCount; i++) boundary.transform.GetChild(i).gameObject.SetActive(true);
        int numOfPoints = 50;
        float radius = MaterialManager.GetMaxLength(currentMaterial);
        Transform boundParent = boundary.transform.GetChild(0);
        Vector3 newPos = new Vector3(center.x, center.y, 105);
        boundParent.position = newPos;

        for (int i = 0; i < numOfPoints; i++) {
            float x = radius * Mathf.Sin((float) i / numOfPoints * 2 * Mathf.PI);
            float y = radius * Mathf.Cos((float) i / numOfPoints * 2 * Mathf.PI);
            Debug.Log(new Vector3(x, y, 0));
            Instantiate(boundaryPoint, new Vector3(x, y, 0) + newPos,
                        Quaternion.identity, boundParent);
        } 
    }

    private void DeactivateBoundary() {
        Transform boundParent = boundary.transform.GetChild(0);
        for (int i = 0; i < boundParent.childCount; i++) GameObject.Destroy(boundParent.GetChild(i).gameObject);
    }

}
