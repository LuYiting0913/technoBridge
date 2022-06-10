using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Stage1Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Transform barParent, pointParent, gridParent;
    private GameObject barTemplate, pointTemplate, fixedPointTemplate;

    public int level = 0;
    private TraceController traceController;
    private SelectionController selectionController;
    private SolidBarInitiator solidbarInitiator;
    private DragController dragController;

    private int currentEditMode = 0;
    // 0 add; 1 select; 2 drag joint; 3: drag copied segment; 4: tracing
    public int currentMaterial = 0;
    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    public bool autoComplete, gridSnap, autoTriangulate;
    // private bool draggingCopied, draggingBackground, tracing;
    public Vector3 backgroundPosition;
    public GameObject slider;
    public GameObject cursor;
    public static float backgroundScale = 1f;

    // private Vector3 originPosition;
    // private Vector2 startPosition; // for dragging copy
    private int gridInterval = 20;
    private Camera myCam;
    private int popUpSec = 1;
    // private Point currentPointDragging;

    private ToggleButton select, drag, trace, steel, wood, pavement;
    private GameObject popupToolBar;

    public Vector2 startPoint, endPoint, curPoint;
    public bool isPointerDown = false;

    // delegates
    public delegate void ModeChangeEventHandler(object source, int i);
    public event ModeChangeEventHandler ModeChanged;

    protected virtual void OnModeChanged() {
        if (ModeChanged != null) ModeChanged(this, currentEditMode);
    }

    public delegate void ClickEventHandler(object source, Stage1Controller e);
    public event ClickEventHandler Pressed;
    public event ClickEventHandler Released;
    public event ClickEventHandler Dragged;

    protected virtual void OnPressed() {
        if (Pressed != null) Pressed(this, this);
    }

    protected virtual void OnReleased() {
        if (Released != null) Released(this, this);
    }

    protected virtual void OnDragged() {
        if (Released != null) Dragged(this, this);
    }

    private void InitDelegates() {
        ModeChanged += selectionController.OnModeChanged; 
        Pressed += selectionController.OnPressed;
        Released += selectionController.OnReleased;
        Dragged += selectionController.OnDragged;

        ModeChanged += traceController.OnModeChanged;
        Pressed += traceController.OnPressed;
        Released += traceController.OnReleased;
        Dragged += traceController.OnDragged;

        ModeChanged += solidbarInitiator.OnModeChanged;        
        Pressed += solidbarInitiator.OnPressed;
        Released += solidbarInitiator.OnReleased;
        Dragged += solidbarInitiator.OnDragged;

        ModeChanged += dragController.OnModeChanged;        
        Pressed += dragController.OnPressed;
        Released += dragController.OnReleased;
        Dragged += dragController.OnDragged;
    }

    public void Start() {
        myCam = Camera.main;
        selectionController = SelectionController.GetInstance();
        traceController = TraceController.GetInstance();
        solidbarInitiator = SolidBarInitiator.GetInstance();
        dragController = DragController.GetInstance();
        
        InitDelegates();

        backgroundScale = Levels.GetBackgroundScale(level);
        backgroundPosition = Levels.GetBackgroundPosition(level);
        
        Levels.InitLevel(level);
        pointTemplate = PrefabManager.GetPoint2DTemplate();
        fixedPointTemplate = PrefabManager.GetFixedPoint2DTemplate();  
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);

        select = GameObject.Find("Select").GetComponent<ToggleButton>();
        drag = GameObject.Find("Drag").GetComponent<ToggleButton>();
        trace = GameObject.Find("TraceTool").GetComponent<ToggleButton>();
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
        startPoint = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
        isPointerDown = true;
        OnPressed();
    }

    public void OnPointerUp(PointerEventData eventData) {
        endPoint = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
        isPointerDown = false;
        OnReleased();
    }

    public void Update() {
        existingBars = AssetManager.GetAllBars();
        existingPoints = AssetManager.GetAllPoints();
        
        foreach (SolidBar bar in existingBars) {
            if (bar != null) bar.RenderSolidBar(backgroundScale);
        }

        if (isPointerDown) {
            Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            curPoint = SnapToGrid(cursor);
            OnDragged();
        }
    }

    public void SelectMode() {
        Debug.Log("select mode");
        currentEditMode = 1;
        TurnOffAll();
        select.ToggleSprite();
        popupToolBar.transform.GetChild(0).gameObject.SetActive(true);
        selectionController.ClearAll();
        OnModeChanged();
    }

    private void AddMode() {
        Debug.Log("add mode");
        currentEditMode = 0;
        OnModeChanged();
        TurnOffAll();
        selectionController.ClearAll();
        OnModeChanged();
    }

    public void DragMode() {
        Debug.Log("drag mode");
        currentEditMode = 2;
        TurnOffAll();
        drag.ToggleSprite();
        selectionController.ClearAll();
        OnModeChanged();
    }

    
    public void TracingMode() {
        currentEditMode = 4;
        TurnOffAll();
        trace.ToggleSprite();
        OnModeChanged();
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

    public void UpdateBackgroundInfo() {
        backgroundScale = slider.GetComponent<Slider>().value;
        transform.localScale = new Vector3(backgroundScale, backgroundScale, transform.localScale.z);
        AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
    }

    private Vector2 WorldToCanvas(Vector2 v) {
        return v - new Vector2(backgroundPosition.x, backgroundPosition.y);
    } 

    public void TurnOffAll() {
        select.TurnOff();
        drag.TurnOff();
        steel.TurnOff();
        wood.TurnOff();
        pavement.TurnOff();
        trace.TurnOff();
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

    public void ClosePopupToolBar(int i) {
        popupToolBar.transform.GetChild(i).gameObject.SetActive(false);
        //popupToolBar.transform.GetChild(1).gameObject.SetActive(false);
        AddMode();
    }

    private void InstantiateGrid() {
        int height = (int) GetComponentInParent<RectTransform>().rect.height;
        int width = (int) GetComponentInParent<RectTransform>().rect.width;
        GameObject horizontalGrid = PrefabManager.GetGridLine();
        GameObject verticalGrid = PrefabManager.GetGridLine();
        horizontalGrid.transform.localScale = new Vector3(width, 1, 1);
        Color darkGrey = new Color(50 / 255.0f, 50 / 255.0f, 50 / 255.0f, 100 / 255.0f);
        Color lightGrey = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
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
        verticalGrid.transform.localScale = new Vector3(1, height, 1);
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

    public void UpdateCursor(Vector2 v) {
        cursor.transform.GetChild(0).transform.position = new Vector3(v.x, 0, -1);
        cursor.transform.GetChild(1).transform.position = new Vector3(0, v.y, -1);
    }

    public void ActivateCursor() {
        for (int i = 0; i < cursor.transform.childCount; i++) cursor.transform.GetChild(i).gameObject.SetActive(true);
    }

    public void DeactivateCursor() {
        for (int i = 0; i < cursor.transform.childCount; i++) cursor.transform.GetChild(i).gameObject.SetActive(false);
    }

}
