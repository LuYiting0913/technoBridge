using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point3D : MonoBehaviour {
    private List<SolidBar3D> connectedBars = new List<SolidBar3D>(); 
    private Vector3 pointId;
    private bool isStantionary = false;

    public void Start() {
        pointId = transform.position;
    }

    public void SetId(Vector3 v) {
        pointId = v;
    }

    public Vector3 GetId() {
        return pointId;
    }

    public void AddConnected3DBar(SolidBar3D bar) {
        connectedBars.Add(bar);
    }

    // public void setStationary() {
    //     this.GetComponent<RigidBody>().isKinemtic = true;
    // }

    public int ConnectedBarCount() {
        return connectedBars.Count;
    }
    
}

