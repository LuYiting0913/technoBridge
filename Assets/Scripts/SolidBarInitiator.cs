using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolidBarInitiator : MonoBehaviour, IPointerDownHandler {
    private bool startedInit = false;
    public GameObject barTemplate;
    private SolidBar currentBar;
    public Transform barParent;

    public GameObject pointTemplate;
    public Point beginPoint;
    public Point endPoint;
    public Transform pointParent;

    public void OnPointerDown(PointerEventData eventData) {
        if (!startedInit) {
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

    // create the beginPoint
    private void InitializeBar(Vector2 headPos) {
        startedInit = true;
        currentBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        
        // check if beginPoint already exists
        if (PointManager.HasPoint(headPos)) {
            //Debug.Log("Land on old vert");
            beginPoint = PointManager.GetPoint(headPos);
            currentBar.SetHead(beginPoint.transform.position);
        } else {
            beginPoint = Instantiate(pointTemplate, headPos, Quaternion.identity, pointParent).GetComponent<Point>();
            PointManager.AddPoint(headPos, beginPoint);
            currentBar.SetHead(headPos);
        }
        
        endPoint = Instantiate(pointTemplate, headPos, Quaternion.identity, pointParent).GetComponent<Point>();
    }

    private void FinalizeBar(Vector2 tailPos) {
        startedInit = false;
        Vector2 cutOffVector = currentBar.CutOff(tailPos);

        // check if endPoint already exists
        if (PointManager.HasPoint(cutOffVector)) {
            Destroy(endPoint.gameObject);
            endPoint = PointManager.GetPoint(cutOffVector);
        } else {
            endPoint.transform.position = cutOffVector; 
            PointManager.AddPoint(cutOffVector, endPoint);
        }

        beginPoint.AddConnectedBar(currentBar);
        endPoint.AddConnectedBar(currentBar);
        SolidBarManager.AddBar(currentBar);
        // continue to the next bar
        InitializeBar(endPoint.transform.position);
    }

    private void DeleteBar() {
        Destroy(currentBar.gameObject);
        if (beginPoint.isSingle()) {
            Destroy(beginPoint.gameObject);
            PointManager.DeletePoint(beginPoint);
        }
        if (endPoint.isSingle()) {
            Destroy(endPoint.gameObject);
            PointManager.DeletePoint(endPoint);
        }
        startedInit = false;
    }

    public void Update() {
        Vector2 cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (startedInit && !Input.GetMouseButton(0)) {
            // cut off the bar at a maximun length
            Vector2 cutOffVector = currentBar.CutOff(cursor);
            // Debug.Log(cutOffVector);
            // Debug.Log(currentBar.GetHead());
            // Debug.Log(cursor);
            // Debug.Log("-----------------------");
            endPoint.transform.position = cutOffVector;
            currentBar.UpdateSolidBar(cutOffVector);
        } //else if (startedInit && Input.GetMouseButton(0)) {
         //   FinalizeBar(v);
        //}
        
    }
}
