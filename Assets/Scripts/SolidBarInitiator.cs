using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SolidBarInitiator : MonoBehaviour {
    private static SolidBarInitiator m_Instance;
    private bool isActive, creating;

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
    private Vector3 backgroundPosition;
    private float backgroundScale;

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
        // Vector3 temp = e.WorldToCanvas(new Vector3(e.GetStartPoint().x, e.GetStartPoint().y, 0));
        Vector3 temp = new Vector3(e.GetStartPoint().x, e.GetStartPoint().y, 0);

        if (isActive && !e.IsOutOfMoney() && AssetManager.HasPointInWorld(temp)) {
            creating = true;
            currentMaterial = e.GetCurrentMaterial();
            backgroundPosition = e.backgroundPosition;
            backgroundScale = Stage1Controller.backgroundScale;
            e.ActivateCursor();
            InitializeBar(temp, e.GetCurrentMaterial(), e.pointParent, e.barParent);
        }
    }

    public void OnReleased(object source, Stage1Controller e) {
        if (isActive && creating) {
            // Debug.Log("add receieved release");
            creating = false;
            e.DeactivateCursor();
            DeactivateBoundary();
            bool landOnHeadPoint = AssetManager.GetPointInWorld(e.GetEndPoint()) == currentBar.head;
            Debug.Log(landOnHeadPoint);
            if (!e.isTutorial) {
                if (!landOnHeadPoint) {
                    FinalizeBar(e.GetEndPoint(), e, e.autoTriangulate, Stage1Controller.backgroundScale);
				    e.GetAudio().PlayBuildSound(e.GetCurrentMaterial());    
                } else {
                    Destroy(endPoint.gameObject);
                    Destroy(currentBar.gameObject);
                }
            } else {
                TutorialController tutorial = GameObject.Find("TutorialController").GetComponent<TutorialController>();
                Point guidePoint = tutorial.FindGuidePoint(e.GetEndPoint());
                // Debug.Log("The guidepoint is " + guidePoint.GetPosition());
                if (guidePoint != null || AssetManager.HasPoint(e.WorldToCanvas(e.GetEndPoint()))) {
    
                    FinalizeBar(e.GetEndPoint(), e, e.autoTriangulate, Stage1Controller.backgroundScale);
					e.GetAudio().PlayBuildSound(e.GetCurrentMaterial());
                } else {
                    Destroy(endPoint.gameObject);
                    Destroy(currentBar.gameObject);
                }
            }
        }

    }

    public void OnDragged(object source, Stage1Controller e) {
        if (isActive && creating) {
            // Debug.Log("add receieved drag");
            float scale = Stage1Controller.backgroundScale;
            Vector2 cutOffVector = currentBar.CutOff(e.GetCurPoint(), scale);
            e.UpdateCursor(cutOffVector);

            if (e.autoComplete && endPoint.ExceedsMaxLength(e.GetCurPoint(), scale)) {
                FinalizeBar(cutOffVector, e, e.autoTriangulate, scale);
                DeactivateBoundary();
                ActivateBoundary(cutOffVector);
                InitializeBar(cutOffVector, e.GetCurrentMaterial(), e.pointParent, e.barParent);
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
        //Vector2 relativeToCanvas = WorldToCanvas(headPos);
        Vector3 head = new Vector3(headPos.x, headPos.y, 0);
        
        beginPoint = AssetManager.GetPointInWorld(headPos);
        currentBar.SetHead(beginPoint);
        endPoint = Instantiate(pointTemplate, pointParent).GetComponent<Point>();
        endPoint.transform.position = head;
        currentBar.SetTail(endPoint);    
        beginPoint.AddConnectedBar(currentBar);
        endPoint.AddConnectedBar(currentBar);

        // Vector2 temp = new Vector2(beginPoint.GetPosition().x, beginPoint.GetPosition().y);
        ActivateBoundary(beginPoint.GetWorldPosition());
    }

    public void FinalizeBar(Vector2 tailPos, Stage1Controller e, bool autoComplete, float scale) {
        startedInit = false;
        Vector3 cutOffVector = currentBar.CutOff(new Vector3(tailPos.x, tailPos.y, 0), scale);

        // check if endPoint already exists
        if (AssetManager.HasPointInWorld(cutOffVector)) {
            Destroy(endPoint.gameObject);
            endPoint = AssetManager.GetPointInWorld(cutOffVector);
            endPoint.AddConnectedBar(currentBar);
        } else {
            endPoint.transform.position = cutOffVector; 
            // endPoint.UpdatePosition();
            AssetManager.AddPoint(endPoint);
        }
        
        if (autoComplete) AutoTriangulate();

        currentBar.SetR(beginPoint, endPoint);
        AssetManager.AddBar(currentBar);
        ClearAll();
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

    // private Vector2 WorldToCanvas(Vector2 v) {
    //     return v - new Vector2(backgroundPosition.x, backgroundPosition.y);
    // } 

    private void ActivateBoundary(Vector2 center) {
        for (int i = 0; i < boundary.transform.childCount; i++) boundary.transform.GetChild(i).gameObject.SetActive(true);
        int numOfPoints = 50;
        float radius = MaterialManager.GetMaxLength(currentMaterial) * backgroundScale;
        Transform boundParent = boundary.transform.GetChild(0);
        Vector3 newPos = new Vector3(center.x, center.y, 0);
        boundParent.position = newPos;

        for (int i = 0; i < numOfPoints; i++) {
            float x = radius * Mathf.Sin((float) i / numOfPoints * 2 * Mathf.PI);
            float y = radius * Mathf.Cos((float) i / numOfPoints * 2 * Mathf.PI);
            Transform pt = Instantiate(boundaryPoint, boundParent).transform;
            pt.localPosition = new Vector3(x, y, 0);
        }  
    }

    private void DeactivateBoundary() {
        Transform boundParent = boundary.transform.GetChild(0);
        for (int i = 0; i < boundParent.childCount; i++) GameObject.Destroy(boundParent.GetChild(i).gameObject);
    }

}
