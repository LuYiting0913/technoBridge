using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointReference {
    private Vector3 pointPosition;
    private bool isStationary = false;

    public static PointReference of(Point p, bool stationary) {
        PointReference reference = new PointReference();
        reference.pointPosition = p.GetPosition();
        reference.isStationary = stationary;
        return reference;
    }

    public void SetPosition(Vector3 position) {
        pointPosition = position;
    }

    public Vector3 GetPosition() {
        return pointPosition;
    }
    
    // public Vector2 GetPosition2D() {
    //     return new Vector2(pointPosition.x, ;
    // }

    public bool IsFixed() {
        return isStationary;
    }

    public void SetFixed() {
        isStationary = true;
    }

}
