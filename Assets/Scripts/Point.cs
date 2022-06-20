using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    //public bool isDragging = false;
    public List<SolidBar> connectedBars = new List<SolidBar>(); 
    // private Vector3 pointPosition;
    private bool isStationary = false;
    private bool isSplit = false;
    private static int threshold = 15;

    public Vector3 GetPosition() {
        return transform.localPosition;
    }
    
    public Vector3 GetWorldPosition() {
        return transform.position;
    }


    public void AddConnectedBar(SolidBar bar) {
        connectedBars.Add(bar);
    }

    public void DeleteConnectedBar(SolidBar bar) {
        connectedBars.Remove(bar);
    }

    public int ConnectedBarCount() {
        return connectedBars.Count;
    }

    public bool ExceedsMaxLength(Vector2 cursor, float scale) {
        bool check = false;
        Vector2 v;
        foreach (SolidBar bar in connectedBars) {
            if (bar != null) {
                if (Contain(bar.head.transform.position)) {
                    v = bar.tail.transform.position;
                } else {
                    v = bar.head.transform.position;
                }
                bool temp = (new Vector2(v.x, v.y) - cursor).magnitude >= MaterialManager.GetMaxLength(bar.GetMaterial()) * scale;
                if (temp) bar.ActivateLimit();
                check = check || temp;
            }
        }
        return check;
    }

    public Vector2 GetReachablePosition(Vector2 origin, Vector2 cursor, float scale) {
        if (ExceedsMaxLength(cursor, scale)) {
            return origin;
        }
        return cursor;
    }

    public void UpdateConnectedBars() {
        // foreach (SolidBar bar in connectedBars) {
        //     // bar.UpdatePosition();
        // }
    }

    public bool IsSingle() {
        return ConnectedBarCount() == 0;
    }

    public bool Contain(Vector3 v) {
        return (GetPosition() - v).magnitude < threshold;
    }

    public bool ContainInWorldPosition(Vector3 v) {
        return (GetWorldPosition() - v).magnitude < threshold;
    }

    public float DistanceTo(Point p) {
        return (p.GetPosition() - GetPosition()).magnitude;
    }

    public bool IsFixed() {
        return isStationary;
    }

    public void SetFixed() {
        isStationary = true;
    }

    public void SetFree() {
        isStationary = false;
    }

    public void SetSplit(bool b) {
        isSplit = b;
        foreach (SolidBar bar in connectedBars) {
            if (bar.head.Contain(GetPosition())) {
                bar.headSplitNum = b ? 0 : -1;
            } else {
                bar.tailSplitNum = b ? 0 : -1;
            }
        }
    }
    
    public bool IsSplit() {
        return isSplit;
    }


    public void InitRigidBody(PointReference p) {
        Rigidbody pointRb = gameObject.GetComponent<Rigidbody>();
        pointRb.isKinematic = p.IsFixed();
    }
        
    public void RemoveConnectedNull() {
        List<SolidBar> temp = new List<SolidBar>();
        foreach (SolidBar b in connectedBars) if (b != null) temp.Add(b);
        connectedBars = temp;
    }    
}
