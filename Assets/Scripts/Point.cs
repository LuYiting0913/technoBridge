using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {
    //public bool isDragging = false;
    public List<SolidBar> connectedBars = new List<SolidBar>(); 
    // private Vector3 pointPosition;
    private bool isStationary = false;
    private static int threshold = 8;

    // public void Start() {
    //     pointPosition = transform.position;
    // }

    // public void UpdatePosition() {
    //     pointPosition = transform.position;
    // }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void AddConnectedBar(SolidBar bar) {
        connectedBars.Add(bar);
    }

    public int ConnectedBarCount() {
        return connectedBars.Count;
    }

    public bool ExceedsMaxLength(Vector2 cursor) {
        bool check = false;
        Vector2 v;
        foreach (SolidBar bar in connectedBars) {
            if (Contain(bar.GetHead())) {
                v = new Vector2(bar.GetTail().x, bar.GetTail().y);
            } else {
                v = new Vector2(bar.GetHead().x, bar.GetHead().y);
            }
            check = check || (v - cursor).magnitude >= MaterialManager.GetMaxLength(bar.GetMaterial());
        }
        return check;
    }

    public void UpdateConnectedBars() {
        foreach (SolidBar bar in connectedBars) {
            // bar.UpdatePosition();
        }
    }

    public bool isSingle() {
        return ConnectedBarCount() == 0;
    }

    public bool Contain(Vector3 v) {
        return (GetPosition() - v).magnitude < threshold;
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

    public void InitRigidBody(PointReference p) {
        Rigidbody pointRb = gameObject.GetComponent<Rigidbody>();
        pointRb.isKinematic = p.IsFixed();
    }
        
    // public void Update() {
    //     pointPosition = transform.position;
    // }
    
}
