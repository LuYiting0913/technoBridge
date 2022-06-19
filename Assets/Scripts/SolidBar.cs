using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SolidBar : MonoBehaviour {
    // private Vector3 head.GetPosition();
    // private Vector3 tail.GetPosition();
    public Point head;
    public Point tail;
    public int headSplitNum = -1;
    public int tailSplitNum = -1;
    private ConfigurableJoint headJoint,tailJoint;

    private Color baseColor;
    public int material; 
    private float maxLoad;
    private Color originColor;
    public bool disabled = false;

    private float hydraulicFactor = 0.5f;

    private SpriteRenderer barRenderer;
    // private float maxLength = 200f; 

    private void Start() {
        headSplitNum = -1;
        tailSplitNum = -1;
    }
    
    private bool isRope() {
        return material == 3 || material == 4;
    }

    public void InitRenderer() {
        Sprite sp = MaterialManager.GetSprite(material);
        barRenderer = GetComponent<SpriteRenderer>();
        barRenderer.drawMode = SpriteDrawMode.Tiled;
        barRenderer.sprite = sp;
    }

    public void RenderSolidBar(float scale) {
        InitRenderer();
        transform.localPosition = (head.GetPosition() + tail.GetPosition()) / 2;// + new Vector3(0, 0, 5);

        Vector3 dir = tail.GetPosition() - head.GetPosition();
        float angle = Vector2.SignedAngle(Vector2.right, new Vector3(dir.x, dir.y));
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        float length = dir.magnitude;
        // not ideal here, change later
        //transform.localScale = new Vector2(length / 10, 5);
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        Transform onSelect = gameObject.transform.GetChild(0);
        Transform exceedLimit = gameObject.transform.GetChild(1);
        collider.size = new Vector2(length / 10, collider.size.y);
        onSelect.localScale = new Vector2(length / 10, onSelect.localScale.y);
        exceedLimit.localScale = new Vector2(length / 20, exceedLimit.localScale.y);
        barRenderer.size = new Vector2(length / 10, barRenderer.size.y);
    }

    public void InitBarHead() {
        headJoint = gameObject.AddComponent<ConfigurableJoint>();
        headJoint.connectedBody = head.GetComponent<Rigidbody>();
        InitJointSetting(headJoint);
        headJoint.anchor = new Vector3(0, -1, 0);
        
        // breakable child
        ConfigurableJoint joint = transform.GetChild(0).gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = head.GetComponent<Rigidbody>();
        InitJointSetting(joint);
        joint.anchor = new Vector3(0, -1, 0);
    }

    public void InitBarTail() {
        tailJoint = gameObject.AddComponent<ConfigurableJoint>();
        tailJoint.connectedBody = tail.GetComponent<Rigidbody>();
        InitJointSetting(tailJoint);
        tailJoint.anchor = new Vector3(0, 1, 0);

        ConfigurableJoint joint = transform.GetChild(1).gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = tail.GetComponent<Rigidbody>();
        InitJointSetting(joint);
        joint.anchor = new Vector3(0, 1, 0);
    }

    private void InitJointSetting(ConfigurableJoint joint) {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.axis = new Vector3(0, 0, 1); 
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
        Vector3 offset = cursor - head.GetWorldPosition();
        return head.GetWorldPosition() + Vector3.ClampMagnitude(offset, GetMaxLength() * scale); 
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
        if (!isRope()) {
            return Math.Max(headJoint.currentForce.x, tailJoint.currentForce.x);
        } else {
            return transform.GetChild(0).GetComponent<ConfigurableJoint>().currentForce.magnitude;
        }
        
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

    public void DisplayStress() {
        if (!isRope()) {
            gameObject.GetComponent<MeshRenderer>().material.color = GetLoadColor();
        } else {
            Color stress = GetLoadColor();
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = stress;
            }
        }
    }

    public void DisplayNormal() {
        if (!isRope()) {
            gameObject.GetComponent<MeshRenderer>().material.color = GetBaseColor();
        } else {
            Color normal = GetBaseColor();
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).GetComponent<MeshRenderer>().material.color = normal;
            }
        }
    }

    public void DisableBar() {
        Destroy(headJoint);
        Destroy(tailJoint);
        gameObject.SetActive(false);
        disabled = true;
    }

    public void ActivateLimit() {
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void DeactivateLimit() {
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void SetHydraulicFactor(float f) {
        hydraulicFactor = f;
    }

    public float GetHydraulicFactor() {
        return hydraulicFactor;
    }




}
