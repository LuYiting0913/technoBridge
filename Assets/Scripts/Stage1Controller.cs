using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stage1Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public GameObject barTemplate;
    public Transform barParent;
    private GameObject pointTemplate;
    private GameObject fixedPointTemplate;
    public Transform pointParent;
    public int level = 0;

    private int currentEditMode = 0;
    // 0 add; 1 select
    private int currentMaterial = 0;
    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    private bool creatingBar = false;
    private bool draggingPoint = false;
    private Camera myCam;
    // private Point currentPointDragging;

    public void Start() {
        myCam = Camera.main;
        if (!Levels.IsInited(level)) Level0.InitLevel();
        pointTemplate = PrefabManager.GetPoint2DTemplate();
        fixedPointTemplate = PrefabManager.GetFixedPoint2DTemplate();    
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);
        
        // render all existing points
        foreach (PointReference p in pointData) {
            Vector3 position = p.GetPosition();
            Point point = null;
            if (p.IsFixed()) {
                point = Instantiate(fixedPointTemplate, position, Quaternion.identity, pointParent).GetComponent<Point>();
                point.SetFixed();
            } else {
                point = Instantiate(pointTemplate, position, Quaternion.identity, pointParent).GetComponent<Point>();
            }
            existingPoints.Add(point);
        }
        AssetManager.Init(existingPoints, null);
        // render all bars
        foreach (SolidBarReference barReference in barData) {
            //Instantiate
            barTemplate = MaterialManager.GetTemplate2D(barReference.GetMaterial());
            SolidBar bar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
            bar.InitRenderer();
           
            bar.SetR(AssetManager.GetPoint(barReference.GetHead3D()), AssetManager.GetPoint(barReference.GetTail3D()));
            //bar.UpdatePosition();
            bar.RenderSolidBar();
            existingBars.Add(bar);
        }
        AssetManager.Init(existingPoints, existingBars);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("ptr down");
        Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dir.z = 0;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1));

        if (currentEditMode == 1 && hit.collider != null) {
            // select mode
            bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
            hit.transform.GetChild(0).gameObject.SetActive(!isActive);
            if (isActive) {
                // deselect
                SelectionController.RemoveFromSelection(hit.transform);
            } else {
                SelectionController.AddToSelection(hit.transform);
            }
        } else if (currentEditMode == 2 && hit.collider != null && hit.transform.GetComponent<Point>() != null) {
            // drag mode
            if (!hit.transform.GetComponent<Point>().IsFixed()) {
                Debug.Log("start drag");
                draggingPoint = true;
                DragController.SelectPoint(hit.transform);
            }     
        } else if (currentEditMode == 0) {
            // add mode
            Debug.Log("start add");
            creatingBar = true;
            SolidBarInitiator.InitializeBar(Camera.main.ScreenToWorldPoint(eventData.position), 
                currentMaterial, pointParent, barParent);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (creatingBar) {
            Debug.Log("pointer up");
            SolidBarInitiator.FinalizeBar(Camera.main.ScreenToWorldPoint(eventData.position));
            creatingBar = false;
        } else if  (draggingPoint) {
            Debug.Log("release point");
            draggingPoint = false;
            DragController.ReleasePoint();
        }
    }

    public void Update() {
        foreach (SolidBar bar in existingBars) {
            bar.RenderSolidBar();
        }

        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (creatingBar) {
            //Debug.Log(SolidBarInitiator.currentBar);
            Vector2 cutOffVector = SolidBarInitiator.currentBar.CutOff(cursor);
            SolidBarInitiator.endPoint.transform.position = cutOffVector;
            //SolidBarInitiator.currentBar.SetTail(cutOffVector);
            SolidBarInitiator.currentBar.RenderSolidBar();
        } else if (draggingPoint) {
            DragController.DragPointTo(cursor);
        }

    }

    
    public void SelectMode() {
        Debug.Log("select mode");
        currentEditMode = 1;
        SelectionController.ClearAll();
    }

    public void AddMode() {
        Debug.Log("add mode");
        currentEditMode = 0;
        SelectionController.ClearAll();
    }

    public void DragMode() {
        Debug.Log("drag mode");
        currentEditMode = 2;
        SelectionController.ClearAll();
    }

    public void SetMaterialWood() {
        currentMaterial = 1;
    }

    public void SetMaterialSteel() {
        currentMaterial = 2;
    }

    public void SetMaterialPavement() {
        currentMaterial = 0;
    }
}
