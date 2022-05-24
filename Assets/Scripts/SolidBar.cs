using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SolidBar : MonoBehaviour {
    private Vector3 headPosition;
    private Vector3 tailPosition;
    public Point head;
    public Point tail;
    private HingeJoint headJoint;
    private HingeJoint tailJoint;
    public int material; 

    private SpriteRenderer barRenderer;
    // private float maxLength = 200f; 

    public void InitRenderer() {
        Sprite sp = MaterialManager.GetSprite(material);
        barRenderer = GetComponent<SpriteRenderer>();
        barRenderer.drawMode = SpriteDrawMode.Tiled;
        barRenderer.sprite = sp;
    }

    public void RenderSolidBar() {
        InitRenderer();
        transform.position = (headPosition + tailPosition) / 2;

        Vector3 dir = tailPosition - headPosition;
        float angle = Vector2.SignedAngle(Vector2.right, new Vector3(dir.x, dir.y));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float length = dir.magnitude;
        // not ideal here, change later
        //transform.localScale = new Vector2(length / 10, 5);
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        Transform child = gameObject.transform.GetChild(0);
        collider.size = new Vector2(length / 10, collider.size.y);
        child.localScale = new Vector2(length / 10,child.localScale.y);
        barRenderer.size = new Vector2(length / 10, barRenderer.size.y);
    }

    public void InitBarHead(Point headPoint) {
        head = headPoint;
        headJoint = gameObject.AddComponent<HingeJoint>();
        headJoint.connectedBody = head.GetComponent<Rigidbody>();
        headJoint.anchor = new Vector3(0, -1, 0);
        headJoint.axis = new Vector3(0, 0, 1); 
        //headJoint.breakForce = MaterialManager.GetIntegrity(material);
    }

    public void InitBarTail(Point tailPoint) {
        tail = tailPoint;
        tailJoint = gameObject.AddComponent<HingeJoint>();
        tailJoint.connectedBody = tail.GetComponent<Rigidbody>();
        tailJoint.anchor = new Vector3(0, 1, 0);
        tailJoint.axis = new Vector3(0, 0, 1); 
        //tailJoint.breakForce = MaterialManager.GetIntegrity(material);
    }

    public void UpdatePosition() {
        headPosition = head.transform.position;
        tailPosition = tail.transform.position;
    }

    public void SetMaterial(int m) {
        material = m;
    }

    public int GetMaterial() {
        return material;
    }
    
    public void SetR(Point point1, Point point2) {
        head = point1;
        tail = point2;
    }

    public void SetTailR(Point point) {
        tail = point;
    }
   // redundant...........
    public void SetHead(Vector3 vector) {
        headPosition = vector;
    }

    public void SetTail(Vector3 vector) {
        tailPosition = vector;
    }

    public Vector3 GetHead() {
        return headPosition;
    }  

    public Vector3 GetTail() {
        return tailPosition;
    }

    public Vector3 CutOff(Vector3 cursor) {
        Vector3 offset = cursor - headPosition;
        return headPosition + Vector3.ClampMagnitude(offset, MaterialManager.GetMaxLength(material)); 
    }

    public Vector2 GetDirection() {
        return new Vector2(tailPosition.x - headPosition.x, tailPosition.y - headPosition.y);
    }

    public float GetLength() {
        return (head.GetPosition() - tail.GetPosition()).magnitude;
    }

    public float GetCurrentTension() {
        return Math.Abs(headJoint.currentForce.x + tailJoint.currentForce.x);
    }

    public float GetCurrentLoad() {
        return GetCurrentTension() / 2000; // max load implement later
    }

    public Color GetLoadColor(Color currentColor) {
        if (GetCurrentLoad() >= 0.3) {
            return new Color(currentColor.r + GetCurrentLoad() * 5, currentColor.g, currentColor.b);
        } 
        return currentColor;
    }

    // public void Update() {
    //     Debug.Log(headJoint.currentForce);
    // }
}
