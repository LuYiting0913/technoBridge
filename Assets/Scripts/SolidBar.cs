using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SolidBar : MonoBehaviour {
    private Vector3 headPosition;
    private Vector3 tailPosition;
    private Point head;
    private Point tail;
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

    public void SetMaterial(int m) {
        material = m;
    }

    public int GetMaterial() {
        return material;
    }

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

    // public void Update() {
    //     Debug.Log(headJoint.currentForce);
    // }
}
