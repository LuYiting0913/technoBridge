using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    //public bool isDragging = false;
    private List<SolidBar> connectedBars = new List<SolidBar>(); 
    private Vector3 pointPosition;
    private bool isStationary = false;
    private static int threshold = 5;

    public void Start() {
        pointPosition = transform.position;
    }

    public void UpdatePosition() {
        pointPosition = transform.position;
    }

    public Vector3 GetPosition() {
        return pointPosition;
    }

    public void AddConnectedBar(SolidBar bar) {
        connectedBars.Add(bar);
    }

    public int ConnectedBarCount() {
        return connectedBars.Count;
    }

    public bool isSingle() {
        return ConnectedBarCount() == 0;
    }

    public bool Contain(Vector3 v) {
        return (pointPosition - v).magnitude < threshold;
    }

    public bool IsFixed() {
        return isStationary;
    }

    public void SetFixed() {
        isStationary = true;
    }
        
    // public void Update() {
    //     pointPosition = transform.position;
    // }
    
}
