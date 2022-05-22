using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stage1Controller : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public GameObject barTemplate;
    public Transform barParent;
    public GameObject pointTemplate;
    public Transform pointParent;
    public int level = 0;

    private int currentEditMode = 0;
    // 0 add; 1 select
    private int currentMaterial = 0;
    private List<Point> existingPoints = new List<Point>();
    private List<SolidBar> existingBars = new List<SolidBar>();
    private bool creatingBar = false;
    private Camera myCam;
    // private Point currentPointDragging;

    public void Start() {
        myCam = Camera.main;
        Level0.InitLevel();
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);
        
        barTemplate = MaterialManager.GetTemplate2D(1);

        // render all existing points
        foreach (PointReference p in pointData) {
            Point point = Instantiate(pointTemplate, p.GetPosition(), Quaternion.identity, pointParent).GetComponent<Point>();
            if (p.IsFixed()) {
                point.SetFixed();
            }
            existingPoints.Add(point);
        }
        // render all bars
        foreach (SolidBarReference barReference in barData) {
            //Instantiate
            SolidBar bar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
            bar.InitRenderer();
            bar.SetHead(barReference.GetHead());
            bar.SetTail(barReference.GetTail());
            bar.RenderSolidBar();
            existingBars.Add(bar);
        }
        AssetManager.Init(existingPoints, existingBars);
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("ptr down");
        if (currentEditMode == 1) {
            // select mode
            Vector3 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dir.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector3(0, 0, 1));
            Debug.Log(myCam.transform.position);
            Debug.Log(dir);
            if (hit.collider != null) {
                Debug.Log(hit.transform.GetChild(0));
                bool isActive = hit.transform.GetChild(0).gameObject.activeSelf;
                hit.transform.GetChild(0).gameObject.SetActive(!isActive);
                if (isActive) {
                    // deselect
                    SelectionController.RemoveFromSelection(hit.transform);
                } else {
                    SelectionController.AddToSelection(hit.transform);
                }
            }
        } else {
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
        }
    }


    public void Update() {
        foreach (SolidBar bar in existingBars) {
            bar.RenderSolidBar();
        }

        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (creatingBar) {
            // cut off the bar at a maximun length
            Vector2 cutOffVector = SolidBarInitiator.currentBar.CutOff(cursor);
            SolidBarInitiator.endPoint.transform.position = cutOffVector;
            SolidBarInitiator.currentBar.SetTail(cutOffVector);
            SolidBarInitiator.currentBar.RenderSolidBar();
        }

    }

    
    public void SelectMode() {
        Debug.Log("select mode");
        currentEditMode = 1;
    }

    public void AddMode() {
        Debug.Log("add mode");
        currentEditMode = 0;
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
