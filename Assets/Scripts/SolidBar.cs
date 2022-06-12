using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SolidBar : MonoBehaviour {
    // private Vector3 head.GetPosition();
    // private Vector3 tail.GetPosition();
    public Point head;
    public Point tail;
    private HingeJoint headJoint;
    private HingeJoint tailJoint;
    private Color baseColor;
    public int material; 
    private float maxLoad;
    private Color originColor;
    public bool disabled = false;

    private SpriteRenderer barRenderer;
    // private float maxLength = 200f; 

    public void InitRenderer() {
        Sprite sp = MaterialManager.GetSprite(material);
        barRenderer = GetComponent<SpriteRenderer>();
        barRenderer.drawMode = SpriteDrawMode.Tiled;
        barRenderer.sprite = sp;
    }

    public void RenderSolidBar(float scale) {
        InitRenderer();
        transform.position = (head.GetPosition() + tail.GetPosition()) / 2 + new Vector3(0, 0, 5);

        Vector3 dir = tail.GetPosition() - head.GetPosition();
        float angle = Vector2.SignedAngle(Vector2.right, new Vector3(dir.x, dir.y));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float length = dir.magnitude;
        // not ideal here, change later
        //transform.localScale = new Vector2(length / 10, 5);
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        Transform onSelect = gameObject.transform.GetChild(0);
        Transform exceedLimit = gameObject.transform.GetChild(1);
        collider.size = new Vector2(length / 10 / scale, collider.size.y);
        onSelect.localScale = new Vector2(length / 10 / scale, onSelect.localScale.y);
        exceedLimit.localScale = new Vector2(length / 20 / scale, exceedLimit.localScale.y);
        barRenderer.size = new Vector2(length / 10 / scale, barRenderer.size.y);
    }

    public void InitBarHead() {
        headJoint = gameObject.AddComponent<HingeJoint>();
        headJoint.connectedBody = head.GetComponent<Rigidbody>();
        headJoint.anchor = new Vector3(0, -1, 0);
        headJoint.axis = new Vector3(0, 0, 1); 
        //headJoint.breakForce = MaterialManager.GetIntegrity(material);
    }

    public void InitBarTail() {
        tailJoint = gameObject.AddComponent<HingeJoint>();
        tailJoint.connectedBody = tail.GetComponent<Rigidbody>();

        tailJoint.anchor = new Vector3(0, 1, 0);
        tailJoint.axis = new Vector3(0, 0, 1); 
        //tailJoint.breakForce = MaterialManager.GetIntegrity(material);
    }


    public void SetMaterial(int m) {
        material = m;
        maxLoad = MaterialManager.GetIntegrity(m);
    }

    public int GetMaterial() {
        return material;
    }
    
    public void SetR(Point point1, Point point2) {
        head = point1;
        tail = point2;
    }

    public void InitTemp(Point point1, Point point2) {
        head = point1;
        tail = point2;
        if (head != null) InitBarHead();
        if (tail != null) InitBarTail();
    }

    public void SetTailR(Point point) {
        tail = point;
    }
   // redundant...........
    public void SetHead(Point point) {
        head = point;
    }

    public void SetTail(Point point) {
        tail = point;
    }

    public Vector3 GetHead() {
        return head.GetPosition();
    }  

    public Vector3 GetTail() {
        return tail.GetPosition();
    }

    public Vector3 GetPosition() {
        return (GetHead() + GetTail()) / 2;
    }

    public Vector3 CutOff(Vector3 cursor, float scale) {
        Vector3 offset = cursor - head.GetPosition();
        return head.GetPosition() + Vector3.ClampMagnitude(offset, GetMaxLength() * scale); 
    }

    public Vector2 GetDirection() {
        return new Vector2(tail.GetPosition().x - head.GetPosition().x, tail.GetPosition().y - head.GetPosition().y);
    }

    public float GetMaxLength() {
        return MaterialManager.GetMaxLength(material);
    }

    public float GetLength() {
        return (head.GetPosition() - tail.GetPosition()).magnitude;
    }

    public int CalculateCost() {
        return (int) (GetLength() * MaterialManager.GetMaterialCost(material));
    }

    public float GetCurrentTension() {
        return Math.Max(headJoint.currentForce.x, tailJoint.currentForce.x);
    }

    public float GetCurrentLoad() {
        return GetCurrentTension() / maxLoad; // max load implement later
    }

    public void SetBaseColor(Color color) {
        baseColor = color;
    }
     
    public Color GetBaseColor() {
        return baseColor;
    }

    // amend later
    public Color GetLoadColor() {
        float load = GetCurrentLoad();
        if (load < 0.5) {
            return new Color(load * 2, 1, 0);
        } else if (load < 1) {
            return new Color(1, 2 - load * 2, 0);
        } else {
            return new Color(1, 0, 0);
        }
    }

    public void DisableBar() {
        Destroy(headJoint);
        Destroy(tailJoint);
        // this.GetComponent<BoxCollider>().enabled = false;
        //this.GetComponent<MeshRenderer>().enabled = false;
        disabled = true;
    }

    public void ActivateLimit() {
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void DeactivateLimit() {
        transform.GetChild(1).gameObject.SetActive(false);
    }

}
