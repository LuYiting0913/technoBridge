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
        Debug.Log("Detected");
        if (!startedInit) {
            // the bar is not initialized
            InitializeBar(Camera.main.ScreenToWorldPoint(eventData.position));
        } else {
            // the beginPoint is initialized
            if (eventData.button == PointerEventData.InputButton.Left) {
                FinalizeBar(Camera.main.ScreenToWorldPoint(eventData.position));
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                DeleteBar();
            }
        }
    }

    // create the beginPoint
    private void InitializeBar(Vector2 headPos) {
        startedInit = true;
        currentBar = Instantiate(barTemplate, barParent).GetComponent<SolidBar>();
        currentBar.headPosition = headPos;
        beginPoint = Instantiate(pointTemplate, headPos, Quaternion.identity, pointParent).GetComponent<Point>();
        endPoint = Instantiate(pointTemplate, headPos, Quaternion.identity, pointParent).GetComponent<Point>();
    }

    private void FinalizeBar(Vector2 tailPos) {
        startedInit = false;
        beginPoint.connectedBars.Add(currentBar);
        endPoint.connectedBars.Add(currentBar);
        //endPoint.transform.position = tailPos;
        // continue to the next bar
        InitializeBar(endPoint.transform.position);
    }

    private void DeleteBar() {
        Destroy(currentBar.gameObject);
        if (beginPoint.connectedBars.Count == 0) {
            Destroy(beginPoint.gameObject);
        }
        if (endPoint.connectedBars.Count == 0) {
            Destroy(endPoint.gameObject);
        }
        startedInit = false;
    }

    public void Update() {
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (startedInit && !Input.GetMouseButton(0)) {
            endPoint.transform.position = v;
            currentBar.UpdateSolidBar(v);
        } //else if (startedInit && Input.GetMouseButton(0)) {
         //   FinalizeBar(v);
        //}
        
    }
}
