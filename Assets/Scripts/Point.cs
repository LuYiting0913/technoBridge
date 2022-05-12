using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    //public bool isDragging = false;
    private List<SolidBar> connectedBars = new List<SolidBar>(); 
    private Vector2 pointId;
    private bool isStantionary;

    public void Start() {
        pointId = transform.position;
        if (!PointManager.HasPoint(pointId)) {
            PointManager.AddPoint(pointId, this);
        }
    }

    public Vector2 GetId() {
        return pointId;
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
        
    public void Update() {

    }
    
}
