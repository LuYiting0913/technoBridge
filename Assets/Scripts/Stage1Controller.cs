using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    // 0 add; 1 select; 2 drag joint; 3: drag copied segment
    private int currentMaterial = 0;
    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    private bool creatingBar = false;
    private bool draggingPoint = false;
    private bool draggingSelection = false;
    private bool autoComplete = false;
    private bool gridSnap = false;
    private bool autoTriangulate = false;
    private bool draggingCopied = false;
    private bool draggingBackground = false;
    private Vector3 backgroundPosition;
    public GameObject slider;
    public float backgroundScale = 1f;

    private Vector3 originPosition;
    private Vector2 startPosition; // for dragging copy
    private int gridInterval = 20;
    private Camera myCam;
    private int popUpSec = 1;
    // private Point currentPointDragging;

    private ToggleButton select, drag, steel, wood, pavement;
    private GameObject popupToolBar;
    
    //public Transform backgroundCanvas;

    public void Start() {
        myCam = Camera.main;
        backgroundScale = Levels.GetBackgroundScale(level);
        backgroundPosition = Levels.GetBackgroundPosition(level);
        
        if (!Levels.IsInited(level)) Level0.InitLevel();
        pointTemplate = PrefabManager.GetPoint2DTemplate();
        fixedPointTemplate = PrefabManager.GetFixedPoint2DTemplate();  
        //pointTemplate.transform.localScale = ScaleVector(pointTemplate.transform.localScale);
        //fixedPointTemplate.transform.localScale = ScaleVector(fixedPointTemplate.transform.localScale);
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);

        select = GameObject.Find("Select").GetComponent<ToggleButton>();
        drag = GameObject.Find("Drag").GetComponent<ToggleButton>();
        pavement = GameObject.Find("Pavement").GetComponent<ToggleButton>();
        wood = GameObject.Find("Wood").GetComponent<ToggleButton>();
        steel = GameObject.Find("Steel").GetComponent<ToggleButton>();
        popupToolBar = GameObject.Find("PopupToolBar");
        
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

            Point head = AssetManager.GetPoint(barReference.GetHead3D());
            Point tail = AssetManager.GetPoint(barReference.GetTail3D());
            head.AddConnectedBar(bar);
            tail.AddConnectedBar(bar);
           
            bar.SetR(head, tail);

            bar.RenderSolidBar(backgroundScale);
            existingBars.Add(bar);
        }
        AssetManager.Init(existingPoints, existingBars);
        AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
        // Init grid lines
        InstantiateGrid();

        transform.localScale = new Vector3(backgroundScale, backgroundScale, transform.localScale.z);
        transform.position = new Vector3(backgroundPosition.x, backgroundPosition.y, transform.position.z);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("ptr down");
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1));

        if (currentEditMode == 1) {
            // select mode
            if (hit.collider != null) {
                SelectionController.ToggleIndividual(hit);
            } else {
                // box select
                draggingSelection = true;
                SelectionController.InitFirstCorner(cursor);
            }

        } else if (currentEditMode == 2 && hit.collider != null && hit.transform.GetComponent<Point>() != null) {
            // drag mode
            if (!hit.transform.GetComponent<Point>().IsFixed()) {
                draggingPoint = true;
                DragController.SelectPoint(hit.transform);
            }     
        } else if (currentEditMode == 0) {
            // add mode
            creatingBar = true;
            Vector2 position = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
            SolidBarInitiator.InitializeBar(position, currentMaterial, pointParent, barParent);
        } else if (currentEditMode == 3) {
            startPosition = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
            originPosition = GameObject.Find("CopiedParent").transform.position;
            draggingCopied = true;
        } else if (currentEditMode == 2 && hit.collider == null) {
            //dragging background
            draggingBackground = true;
            startPosition = cursor;
            backgroundPosition = gameObject.transform.position;
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (currentEditMode == 1 && draggingSelection) {
            draggingSelection = false;
            SelectionController.FinalizeBoxSelection();
        } else if (creatingBar) {
            Vector2 position = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
            SolidBarInitiator.FinalizeBar(position, autoTriangulate, backgroundScale);
            creatingBar = false;
        } else if (draggingPoint) {
            draggingPoint = false;
            DragController.ReleasePoint();
        } else if (currentEditMode == 3) {
            draggingCopied = false;
            SelectionController.SnapToExistingPoint();
        } else if (draggingBackground) {
            draggingBackground = false;
            backgroundPosition = transform.position;
            AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
        }
    }

    public void Update() {
        existingBars = AssetManager.GetAllBars();
        existingPoints = AssetManager.GetAllPoints();
        
        foreach (SolidBar bar in existingBars) {
            bar.RenderSolidBar(backgroundScale);
        }

        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 snapedCursor = SnapToGrid(cursor);
    

        if (currentEditMode == 1 && draggingSelection) {
            SelectionController.InitSecondCorner(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            SelectionController.RenderSelectionBox();
        } else if (creatingBar) {
            Vector2 cutOffVector = SolidBarInitiator.currentBar.CutOff(snapedCursor, backgroundScale);
            if (autoComplete && SolidBarInitiator.endPoint.ExceedsMaxLength(snapedCursor, backgroundScale)) {
                SolidBarInitiator.FinalizeBar(cutOffVector, autoTriangulate, backgroundScale);
                SolidBarInitiator.InitializeBar(cutOffVector, currentMaterial, barParent, pointParent);
            } else { 
                SolidBarInitiator.endPoint.transform.position = cutOffVector;
                SolidBarInitiator.currentBar.RenderSolidBar(backgroundScale);
            }
        } else if (draggingPoint) {
            DragController.DragPointTo(snapedCursor, backgroundScale);
        } else if (currentEditMode == 3 && draggingCopied) {
            Vector2 dir = snapedCursor - startPosition;
            Vector3 newPosition = originPosition + new Vector3(dir.x, dir.y, 0);
            GameObject.Find("CopiedParent").transform.position = newPosition;
        } else if (draggingBackground) {
            Vector2 dir = cursor - startPosition;
            gameObject.transform.position = backgroundPosition + new Vector3(dir.x, dir.y, 0);
        }
    }

    private Vector3 PositionToCanvas(Vector3 pos) {
        Vector3 temp = (pos - backgroundPosition) / backgroundScale;
        return new Vector3(temp.x, temp.y, pos.z);
    }

    private Vector3 ScaleVector(Vector3 v) {
        return new Vector3(v.x * backgroundScale, v.y * backgroundScale, v.z);
    }
    
    public void SelectMode() {
        Debug.Log("select mode");
        currentEditMode = 1;
        TurnOffAll();
        select.ToggleSprite();
        popupToolBar.transform.GetChild(0).gameObject.SetActive(true);
        SelectionController.ClearAll();
    }

    private void AddMode() {
        Debug.Log("add mode");
        currentEditMode = 0;
        TurnOffAll();
        SelectionController.ClearAll();
    }

    public void DragMode() {
        Debug.Log("drag mode");
        currentEditMode = 2;
        TurnOffAll();
        drag.ToggleSprite();
        SelectionController.ClearAll();
    }

    public void DragCopiedMode() {
        currentEditMode = 3;
        popupToolBar.transform.GetChild(0).gameObject.SetActive(false);
        popupToolBar.transform.GetChild(1).gameObject.SetActive(true);
        // TurnOffAll();
    }

    public void SetMaterialWood() {
        AddMode();
        currentMaterial = 1;
        wood.ToggleSprite();
    }

    public void SetMaterialSteel() {
        AddMode();
        currentMaterial = 2;
        steel.ToggleSprite();
    }

    public void SetMaterialPavement() {
        AddMode();
        currentMaterial = 0;
        pavement.ToggleSprite();
    }

    public void UpdateBackgroundScale() {
        backgroundScale = slider.GetComponent<Slider>().value;
        transform.localScale = new Vector3(backgroundScale, backgroundScale, transform.localScale.z);
        AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
    }

    public void TurnOffAll() {
        select.TurnOff();
        drag.TurnOff();
        steel.TurnOff();
        wood.TurnOff();
        pavement.TurnOff();
        popupToolBar.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ToggleAutoComplete() {
        autoComplete = ! autoComplete;
        ToggleButton button = GameObject.Find("AutoComplete").GetComponent<ToggleButton>();
        button.ToggleSprite();
    }

    public void ToggleAutoTriangulate() {
        autoTriangulate = ! autoTriangulate;
        ToggleButton button = GameObject.Find("AutoTriangulate").GetComponent<ToggleButton>();
        button.ToggleSprite();
    }

    public void ToggleGridSnap() {
        gridSnap = ! gridSnap;
        gridParent.gameObject.SetActive(gridSnap);
        ToggleButton button = GameObject.Find("GridSnap").GetComponent<ToggleButton>();
    	button.ToggleSprite();
    }

    public void ClosePopupToolBar() {
        popupToolBar.transform.GetChild(0).gameObject.SetActive(false);
        popupToolBar.transform.GetChild(1).gameObject.SetActive(false);
        AddMode();
    }

    private void InstantiateGrid() {
        int height = (int) GetComponentInParent<RectTransform>().rect.height;
        int width = (int) GetComponentInParent<RectTransform>().rect.width;
        GameObject horizontalGrid = PrefabManager.GetGridLine();
        GameObject verticalGrid = PrefabManager.GetGridLine();
        horizontalGrid.transform.localScale = new Vector3(width, 3, 3);
        Color darkGrey = new Color(75 / 255.0f, 75 / 255.0f, 75 / 255.0f, 100 / 255.0f);
        Color lightGrey = new Color(150 / 255.0f, 150 / 255.0f, 150 / 255.0f, 100 / 255.0f);
        // horizontal grid
        for (int i = (int) - height / 2 / gridInterval; i <= height / 2 / gridInterval; i++) {
            Vector3 position = new Vector3(0, i * gridInterval, 100);
            //Color c = horizontalGrid.GetComponent<SpriteRenderer>().color;
            SpriteRenderer line = Instantiate(horizontalGrid, position, Quaternion.identity, gridParent).GetComponent<SpriteRenderer>();
            if (i % 5 == 0) {
                line.color = darkGrey;
            } else {
                line.color = lightGrey;
            }
        }


        //vertical grid
        verticalGrid.transform.localScale = new Vector3(3, height, 3);
        for (int i = (int) - width / 2 / gridInterval; i <= width / 2 / gridInterval; i++) {
            Vector3 position = new Vector3(i * gridInterval, 0, 100);
            SpriteRenderer line = Instantiate(verticalGrid, position, Quaternion.identity, gridParent).GetComponent<SpriteRenderer>();
            if (i % 5 == 0) {
                line.color = darkGrey;
            } else {
                line.color = lightGrey;
            }
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
