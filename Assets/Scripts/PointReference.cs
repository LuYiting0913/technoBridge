using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointReference {
    private Vector3 pointPosition;
    private bool isStationary = false;
    private bool isSplit;

    public static PointReference of(Point p, bool stationary) {
        PointReference reference = new PointReference();
        reference.pointPosition = p.GetPosition();
        reference.isStationary = p.IsFixed();
        reference.isSplit = p.IsSplit();
        return reference;
    }

    public void SetPosition(Vector3 position) {
        pointPosition = position;
    }

    public Vector3 GetPosition() {
        return pointPosition;
    }
    
    public bool IsSplit() {
        return isSplit;
    }

    public bool IsFixed() {
        return isStationary;
    }

    public void SetFixed() {
        isStationary = true;
    }

}
