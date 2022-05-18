using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolidBarInitiator : MonoBehaviour, IPointerDownHandler {
    private bool startedInit = false;
    private SolidBar currentBar;
    private Point beginPoint;
    private Point endPoint;
    private int currentMaterial = 1;

    public int level;
    public GameObject barTemplate;
    public Transform barParent;
    public GameObject pointTemplate;
    public Transform pointParent;

    public void Start() {
        Level0.InitLevel();
        List<PointReference> pointData = Levels.GetPointData(level);
        List<SolidBarReference> barData = Levels.GetBarData(level);
        List<Point> existingPoints = new List<Point>();
        List<SolidBar> existingBars = new List<SolidBar>();
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
        if (!startedInit && eventData.button == PointerEventData.InputButton.Left) {
            // the bar is not initialized
            InitializeBar(Camera.main.ScreenToWorldPoint(eventData.position));
        } else {
            // the beginPoint is initialized
            if (eventData.button == PointerEventData.InputButton.Left) {
                FinalizeBar(Camera.main.ScreenToWorldPoint(eventData.position));
                //FinalizeBar(endPoint.transform.position);
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                DeleteBar();
            }
        }
    }

    private void InitializeBar(Vector2 headPos) {
        startedInit = true;
        currentBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        Vector3 head = new Vector3(headPos.x, headPos.y, 0);
        
        // check if beginPoint already exists
        if (AssetManager.HasPoint(head)) {
            beginPoint = AssetManager.GetPoint(head);
            currentBar.SetHead(beginPoint.GetPosition());
        } else {
            beginPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
            AssetManager.AddPoint(beginPoint);
            currentBar.SetHead(head);
        }
        endPoint = Instantiate(pointTemplate, head, Quaternion.identity, pointParent).GetComponent<Point>();
    }

    private void FinalizeBar(Vector2 tailPos) {
        startedInit = false;
        Vector3 cutOffVector = currentBar.CutOff(new Vector3(tailPos.x, tailPos.y, 0));

        // check if endPoint already exists
        if (AssetManager.HasPoint(cutOffVector)) {
            Destroy(endPoint.gameObject);
            endPoint = AssetManager.GetPoint(cutOffVector);
        } else {
            endPoint.transform.position = cutOffVector; 
            endPoint.UpdatePosition();
            AssetManager.AddPoint(endPoint);
        }

        beginPoint.AddConnectedBar(currentBar);
        endPoint.AddConnectedBar(currentBar);
        AssetManager.AddBar(currentBar);
        
        // commit changes to the central class
        Levels.UpdateLevelData(0, AssetManager.GeneratePointReference(), AssetManager.GenerateBarReference());
        // continue to the next bar
        InitializeBar(endPoint.transform.position);
    }

    private void DeleteBar() {
        startedInit = false;
        Destroy(currentBar.gameObject);
        //AssetManager.DeleteBar(currentBar);
        if (beginPoint.isSingle() && !beginPoint.IsFixed()) {
            Destroy(beginPoint.gameObject);
            AssetManager.DeletePoint(beginPoint);
        }
        if (endPoint.isSingle() && !endPoint.IsFixed()) {
            Destroy(endPoint.gameObject);
            AssetManager.DeletePoint(endPoint);
        }
    }

    public void SetMaterialWood() {
        currentMaterial = 1;
        barTemplate = MaterialManager.GetTemplate2D(1);
    }

    public void SetMaterialSteel() {
        currentMaterial = 2;
        barTemplate = MaterialManager.GetTemplate2D(2);
    }

    public void SetMaterialPavement() {
        currentMaterial = 0;
        barTemplate = MaterialManager.GetTemplate2D(0);
    }

    public void Update() {
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (startedInit && !Input.GetMouseButton(0)) {
            // cut off the bar at a maximun length
            Vector2 cutOffVector = currentBar.CutOff(cursor);

            endPoint.transform.position = cutOffVector;
            currentBar.SetTail(cutOffVector);
            currentBar.RenderSolidBar();
        } 
    }
}
