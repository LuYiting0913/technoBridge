using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Stage1Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public GameObject barTemplate;
    public Transform barParent;
    private GameObject pointTemplate;
    private GameObject fixedPointTemplate;
    public Transform pointParent;
    public Transform gridParent;
    public int level = 0;

    private int currentEditMode = 0;
    // 0 add; 1 select
    private int currentMaterial = 0;
    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    private bool creatingBar = false;
    private bool draggingPoint = false;
    private bool draggingSelection = false;
    private bool autoComplete = false;
    private bool gridSnap = false;
    private bool autoTriangulate = false;
    private int gridInterval = 30;
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
            // bar.InitRenderer();

            Point head = AssetManager.GetPoint(barReference.GetHead3D());
            Point tail = AssetManager.GetPoint(barReference.GetTail3D());
            head.AddConnectedBar(bar);
            tail.AddConnectedBar(bar);
           
            bar.SetR(head, tail);

            bar.RenderSolidBar();
            existingBars.Add(bar);
        }
        AssetManager.Init(existingPoints, existingBars);

        // Init grid lines
        InstantiateGrid();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("ptr down");
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1));

        if (currentEditMode == 1) {
            // select mode
            if (hit.collider != null) {
                // individual select
                bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
                hit.transform.GetChild(0).gameObject.SetActive(!isActive);
                if (isActive) {
                    // deselect
                    SelectionController.RemoveFromSelection(hit.transform);
                } else {
                    SelectionController.AddToSelection(hit.transform);
                }
            } else {
                // drag select
                draggingSelection = true;
                SelectionController.InitFirstCorner(cursor);
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
            Vector2 position = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
            SolidBarInitiator.InitializeBar(position, currentMaterial, pointParent, barParent);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (currentEditMode == 1 && draggingSelection) {
            Debug.Log("Select inbox");
            draggingSelection = false;
            SelectionController.FinalizeBoxSelection();
        } else if (creatingBar) {
            Debug.Log("pointer up");
            Vector2 position = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
            SolidBarInitiator.FinalizeBar(position, autoTriangulate);
            creatingBar = false;
        } else if (draggingPoint) {
            Debug.Log("release point");
            draggingPoint = false;
            DragController.ReleasePoint();
        }
    }

    public void Update() {
        existingBars = AssetManager.GetAllBars();
        existingPoints = AssetManager.GetAllPoints();
        
        foreach (SolidBar bar in existingBars) {
            bar.RenderSolidBar();
        }

        Vector2 cursor = SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (currentEditMode == 1 && draggingSelection) {
            SelectionController.InitSecondCorner(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SelectionController.RenderSelectionBox();
        } else if (creatingBar) {
            Vector2 cutOffVector = SolidBarInitiator.currentBar.CutOff(cursor);
            if (autoComplete && SolidBarInitiator.endPoint.ExceedsMaxLength(cursor)) {
                SolidBarInitiator.FinalizeBar(cutOffVector, autoTriangulate);
                SolidBarInitiator.InitializeBar(cutOffVector, currentMaterial, barParent, pointParent);
            } else { 
                SolidBarInitiator.endPoint.transform.position = cutOffVector;
                SolidBarInitiator.currentBar.RenderSolidBar();
            }

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

    public void ToggleAutoComplete() {
        autoComplete = ! autoComplete;
    }

    public void ToggleAutoTriangulate() {
        autoTriangulate = ! autoTriangulate;
    }

    public void ToggleGridSanp() {
        gridSnap = ! gridSnap;
        gridParent.gameObject.SetActive(gridSnap);
    }

    private void InstantiateGrid() {
        int height = (int) GetComponentInParent<RectTransform>().rect.height;
        int width = (int) GetComponentInParent<RectTransform>().rect.width;
        GameObject horizontalGrid = PrefabManager.GetGridLine();
        GameObject verticalGrid = PrefabManager.GetGridLine();
        horizontalGrid.transform.localScale = new Vector3(width, 1, 1);

        // horizontal grid
        for (int i = (int) - height / 2 / gridInterval; i <= height / 2 / gridInterval; i++) {
            Vector3 position = new Vector3(0, i * gridInterval, 100);
            Instantiate(horizontalGrid, position, Quaternion.identity, gridParent);
        }


        //vertical grid
        verticalGrid.transform.localScale = new Vector3(1, height, 1);
        for (int i = (int) - width / 2 / gridInterval; i <= width / 2 / gridInterval; i++) {
            Vector3 position = new Vector3(i * gridInterval, 0, 100);
            Instantiate(verticalGrid, position, Quaternion.identity, gridParent);
        }

        gridParent.gameObject.SetActive(gridSnap);
    }

    private Vector2 SnapToGrid(Vector2 v) {
        if (gridSnap) {
            int x = (int) Math.Round(v.x / gridInterval, 0) * gridInterval;
            int y = (int) Math.Round(v.y / gridInterval, 0) * gridInterval;
            return new Vector2(x, y);
        } else {
            return v;
        }
    }

}
