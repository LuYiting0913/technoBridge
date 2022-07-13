using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Stage1Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public int level = 0;
    public bool isTutorial;
    public int budget;
    private bool outOfMoney;

    private Vector2 startPoint, endPoint, curPoint;
    private bool isPointerDown = false;
    private int currentEditMode = 0;
    // 0 add; 1 select; 2 drag joint; 3: drag copied segment; 4: tracing
    public bool autoComplete, gridSnap, autoTriangulate;
    private int currentMaterial = 0;

    private ToggleButton select, drag, trace, steel, wood, pavement, rope, cable, hydraulic;
    public Transform barParent, pointParent, gridParent;
    private GameObject barTemplate, pointTemplate, fixedPointTemplate;
    private GameObject popupToolBar;

    private TraceController traceController;
    private SelectionController selectionController;
    private SolidBarInitiator solidbarInitiator;
    private DragController dragController;
    private HydraulicInitiator hydraulicInitiator;
    // private SplitBarController splitBarController;
    public AudioManager audioManager;
    public Transform gameInfo;

    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    
    // background management
    public Vector3 backgroundPosition;
    public GameObject slider, cursor, costDisplay;
    public static float backgroundScale = 1f;

    private int gridInterval = 10;
    // private Camera myCam;
    private int popUpSec = 1;


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

        ModeChanged += hydraulicInitiator.OnModeChanged;        
        Pressed += hydraulicInitiator.OnPressed;
        Released += hydraulicInitiator.OnReleased;
        Dragged += hydraulicInitiator.OnDragged;

        ModeChanged += dragController.OnModeChanged;        
        Pressed += dragController.OnPressed;
        Released += dragController.OnReleased;
        Dragged += dragController.OnDragged;

        // Pressed += splitBarController.OnPressed;
    }

    public void Start() {
        Time.timeScale = 1f;
        selectionController = SelectionController.GetInstance();
        traceController = TraceController.GetInstance();
        solidbarInitiator = SolidBarInitiator.GetInstance();
        dragController = DragController.GetInstance();
        hydraulicInitiator = HydraulicInitiator.GetInstance();
        
        InitDelegates();

        // backgroundScale = 1f;
        backgroundScale = slider.GetComponent<Slider>().value;
        backgroundPosition = new Vector3(0,0,0);
        gameInfo.gameObject.SetActive(!Levels.IsInited(level));
        Levels.InitLevel(level);
        costDisplay.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Budget: " + MoneyToString(budget);
        // gameInfo.gameObject.SetActive(Levels.IsInited(level));
        // pointTemplate = PrefabManager.GetPoint2DTemplate();
        // fixedPointTemplate = PrefabManager.GetFixedPoint2DTemplate();  
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);
        // existingPoints = pointData

        select = GameObject.Find("Select").GetComponent<ToggleButton>();
        drag = GameObject.Find("Drag").GetComponent<ToggleButton>();
        trace = GameObject.Find("TraceTool").GetComponent<ToggleButton>();
        pavement = GameObject.Find("Pavement").GetComponent<ToggleButton>();
        wood = GameObject.Find("Wood").GetComponent<ToggleButton>();
        steel = GameObject.Find("Steel").GetComponent<ToggleButton>();
        rope = GameObject.Find("Rope").GetComponent<ToggleButton>();
        cable = GameObject.Find("Cable").GetComponent<ToggleButton>();
        hydraulic = GameObject.Find("Hydraulic").GetComponent<ToggleButton>();
        popupToolBar = GameObject.Find("PopupToolBar");
        
        foreach (PointReference p in pointData) {
            existingPoints.Add(Point.Instantiate2D(p, pointParent));
        }

        AssetManager.Init(existingPoints, null);

        foreach (SolidBarReference barReference in barData) {
            existingBars.Add(SolidBar.Instantiate2D(barReference, barParent));
        }
        
        foreach (Point p in existingPoints) {
            p.SetSplit();
        }

        AssetManager.Init(existingPoints, existingBars);
        AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
        AssetManager.SetBudget(budget);
        InstantiateGrid();

        transform.localScale = new Vector3(backgroundScale, backgroundScale, transform.localScale.z);
        transform.position = new Vector3(backgroundPosition.x, backgroundPosition.y, 0);        
    }

    public void OnPointerDown(PointerEventData eventData) {
        startPoint = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
        // startPoint = WorldToCanvas(startPoint);
        isPointerDown = true;
        // Debug.Log(WorldToCanvas(startPoint));
        OnPressed();
    }

    public void OnPointerUp(PointerEventData eventData) {
        endPoint = SnapToGrid(Camera.main.ScreenToWorldPoint(eventData.position));
        // endPoint = WorldToCanvas(endPoint);
        isPointerDown = false;
        OnReleased();
    }

    public void Update() {
        existingBars = AssetManager.GetAllBars();
        existingPoints = AssetManager.GetAllPoints();
        int cost = 0; 

        foreach (SolidBar bar in existingBars) {
            if (bar != null) {
                bar.RenderSolidBar(backgroundScale);
                cost += bar.CalculateCost();    
            }
        }
        UpdateCost(cost);
        // CheckOutOfMoney(cost);

        if (isPointerDown) {
            Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            curPoint = SnapToGrid(cursor);
            OnDragged();
        }
    }

    // private void CheckOutOfMoney(int c) {
    //     if (c > 1.5f * budget) {
    //         outOfMoney = true;
    //         costDisplay.transform.GetChild(2).gameObject.SetActive(true);
    //     } else {
    //         outOfMoney = false;
    //         costDisplay.transform.GetChild(2).gameObject.SetActive(false);
    //     }
    // }

    public bool IsOutOfMoney() {
        return outOfMoney;
    }

    public Vector2 GetStartPoint() {
        return startPoint;
    } 

    public Vector2 GetEndPoint() {
        return endPoint;
    }

    public Vector2 GetCurPoint() {
        return curPoint;
    }

    public Vector2 GetCurPointInWorld() {
        return curPoint;
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
        TurnOnMaterialToggle(currentMaterial);
        OnModeChanged();
    }

    public void DragCopiedMode() {
        currentEditMode = 3;
        popupToolBar.transform.GetChild(0).gameObject.SetActive(false);
        popupToolBar.transform.GetChild(1).gameObject.SetActive(true);
        TurnOffAll();
    }

    public void SetMaterial(int material) {
        AddMode();
        currentMaterial = material;
        TurnOnMaterialToggle(material);  
    }

    private void TurnOnMaterialToggle(int mat) {
        switch (mat) {
            case 0:
                pavement.ToggleSprite();
                break;
            case 1:
                wood.ToggleSprite();
                break;
            case 2:
                steel.ToggleSprite();
                break;
            case 3:
                rope.ToggleSprite();
                break;
            case 4:
                cable.ToggleSprite();
                break;
            case 5: 
                hydraulic.ToggleSprite();
                break;
            default:
                break;
        }    
    }

    public int GetCurrentMaterial() {
        return currentMaterial;
    }

    public AudioManager GetAudio() {
	    return audioManager;
    }

    public void UpdateBackgroundInfo() {
        backgroundScale = slider.GetComponent<Slider>().value;
        transform.localScale = new Vector3(backgroundScale, backgroundScale, transform.localScale.z);
        AssetManager.UpdateBackground(backgroundPosition, backgroundScale);
    }

    public Vector2 WorldToCanvas(Vector2 v) {
        return (v - new Vector2(backgroundPosition.x, backgroundPosition.y)) / backgroundScale;
    } 

    public void TurnOffAll() {
        select.TurnOff();
        drag.TurnOff();
        steel.TurnOff();
        wood.TurnOff();
        pavement.TurnOff();
        rope.TurnOff();
        cable.TurnOff();
        trace.TurnOff();
        hydraulic.TurnOff();
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

    private void UpdateCost(int c) {
        // Color color = new Color(1, 0, 0);
        costDisplay.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = MoneyToString(c);
        if (c > budget) {
            costDisplay.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = new Color(1, 0, 0);
            costDisplay.transform.GetChild(3).gameObject.SetActive(c <= budget * 2);
            costDisplay.transform.GetChild(2).gameObject.SetActive(c > budget * 2);
            outOfMoney = c > budget * 2;

        } else {
            costDisplay.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().color = new Color(0, 1, 0);
            costDisplay.transform.GetChild(3).gameObject.SetActive(false);
            costDisplay.transform.GetChild(2).gameObject.SetActive(false);
            outOfMoney = false;
        }
        AssetManager.UpdateCost(c);
    }

    private String MoneyToString(int c) {
        String s = "$";
        if (c < 1000) {
            s += c;
        } else {
            s += c / 1000 + ",";
            int r = c % 1000;
            if (r < 100) s += "0";
            if (r < 10) s += "0";
            s += r;
        }
        return s;
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
        cursor.transform.GetChild(0).transform.position = new Vector3(v.x, 0, 0);
        cursor.transform.GetChild(1).transform.position = new Vector3(0, v.y, 0);
    }

    public void ActivateCursor() {
        for (int i = 0; i < cursor.transform.childCount; i++) cursor.transform.GetChild(i).gameObject.SetActive(true);
    }

    public void DeactivateCursor() {
        for (int i = 0; i < cursor.transform.childCount; i++) cursor.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void ToggleGameInfoPage(bool b) {
        gameInfo.gameObject.SetActive(b);
    }

}
