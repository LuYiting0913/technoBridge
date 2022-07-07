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
    public Transform headSplitController, tailSplitController;
    // public Transform hydraulicSlider;
    private ConfigurableJoint headJoint,tailJoint;

    private Color baseColor;
    public int material; 
    private float maxLoad;
    private Color originColor;
    public bool disabled = false;

    public float hydraulicFactor = 0.5f;

    private SpriteRenderer barRenderer;

    public static SolidBar Instantiate2D(SolidBarReference barReference, Transform barParent) {
        GameObject template = MaterialManager.GetTemplate2D(barReference.GetMaterial());
        SolidBar bar = Instantiate(template, barParent).GetComponent<SolidBar>();

        Point head = AssetManager.GetPoint(barReference.GetHead3D());
        Point tail = AssetManager.GetPoint(barReference.GetTail3D());
        head.AddConnectedBar(bar);
        tail.AddConnectedBar(bar);

        
        // Debug.Log("ddd");
        // Debug.Log(bar.headSplitNum);
        // Debug.Log(bar.tailSplitNum);
    
        // bar.InitSplitController();

        bar.SetR(head, tail);
        bar.InitHydraulicParams(barReference.GetHydraulicFactor());

        // some strange op here
        bar.headSplitNum = (barReference.GetHeadSplitNum() + 1) % 2;
        bar.tailSplitNum = (barReference.GetTailSplitNum() + 1) % 2;
        if (head.IsSplit()) bar.ToggleSplitParent(0);
        if (tail.IsSplit()) bar.ToggleSplitParent(1);
        return bar;
    }

    public static SolidBar Instantiate3D(SolidBarReference barReference, int z, Transform barParent, Transform hydraulicParent) {
        if (barReference.GetMaterial() < 3) {
            return Instantiate3DBar(barReference, z, barParent);
        } else if (barReference.GetMaterial() < 5) {
            return Instantiate3DRope(barReference, z, barParent);
        } else {
            return Instantiate3DHydraulic(barReference, z, hydraulicParent);
        }
    }

    private static SolidBar Instantiate3DBar(SolidBarReference bar, int i, Transform parent) {
        Vector3 headPosition = bar.GetHead3D() + new Vector3(0, 0, i);
        Vector3 tailPosition = bar.GetTail3D() + new Vector3(0, 0, i);
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        scaledTemplate.transform.localScale = new Vector3(50, dir.magnitude / 2, 50);

        SolidBar newBar = Instantiate(scaledTemplate, midPoint,  
                            Quaternion.Euler(new Vector3(0, 0, angle)), parent).GetComponent<SolidBar>();

        newBar.SetMaterial(bar.GetMaterial());
        newBar.SetBaseColor(newBar.GetComponent<MeshRenderer>().material.color);

        Point head = bar.GetHeadSplit(AssetManager.GetPoint(headPosition));
        Point tail = bar.GetTailSplit(AssetManager.GetPoint(tailPosition));

        newBar.InitTemp(head, tail);
        return newBar;
    }


    private static SolidBar Instantiate3DRope(SolidBarReference bar, int i, Transform parent) {
        GameObject ropeParent = new GameObject("RopeParent");
        ropeParent.transform.parent = parent;
        SolidBar newRope = ropeParent.AddComponent<SolidBar>();
        
        float maxPerSegment = 20f;
        Vector3 headPosition = bar.GetHead3D() + new Vector3(0, 0, i);
        Vector3 tailPosition = bar.GetTail3D() + new Vector3(0, 0, i);
        Vector3 dir = tailPosition - headPosition;
        float l = dir.magnitude;
        int numberOfSegments = (int) (l / maxPerSegment);
        // Debug.Log(numberOfSegments);

        Point head = bar.GetHeadSplit(AssetManager.GetPoint(headPosition));
        Point tail = bar.GetTailSplit(AssetManager.GetPoint(tailPosition));

        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());
        scaledTemplate.transform.localScale = new Vector3(10, l / numberOfSegments / 1.9f, 10);
        Quaternion rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, bar.GetDirection()));
        
        GameObject previousSegment = head.gameObject;
        for (int j = 1; j <= numberOfSegments; j++) {
            Vector3 tempHead = headPosition + ((float) (j - 1) / numberOfSegments) * dir; 
            Vector3 tempTail = headPosition + ((float) j / numberOfSegments) * dir;
            Vector3 tempPos = (tempHead + tempTail) / 2;
            SolidBar b = Instantiate(scaledTemplate, tempPos, rotation, ropeParent.transform).GetComponent<SolidBar>();
            ConfigurableJoint joint = b.gameObject.GetComponent<ConfigurableJoint>();
            joint.connectedBody = previousSegment.GetComponent<Rigidbody>();
            joint.anchor = new Vector3(0, -0.8f, 0);
            InitRopeJoint(joint); 
            previousSegment = b.gameObject;        
        }
        ConfigurableJoint jt = previousSegment.AddComponent<ConfigurableJoint>();
        jt.connectedBody = tail.GetComponent<Rigidbody>();
        jt.anchor = new Vector3(0, 1, 0);
        InitRopeJoint(jt); 

        newRope.SetMaterial(bar.GetMaterial());
        newRope.SetBaseColor(previousSegment.GetComponent<MeshRenderer>().material.color);

        newRope.SetR(head, tail);

        return newRope;
    }

    private static SolidBar Instantiate3DHydraulic(SolidBarReference barReference, int i, Transform parent) {
        return Instantiate3DBar(barReference, i, parent);
        // b.GetComponent<HydraulicController>().ConvertToHydraulic()
    }

    private static void InitRopeJoint(ConfigurableJoint joint) {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.axis = new Vector3(0, 0, 1); 
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
        collider.size = new Vector2(length / 12, collider.size.y);
        onSelect.localScale = new Vector2(length / 10, onSelect.localScale.y);
        exceedLimit.localScale = new Vector2(length / 20, exceedLimit.localScale.y);
        barRenderer.size = new Vector2(length / 10, barRenderer.size.y);
        // Debug.Log(tailSplitNum);
    }

    public void InitBarHead() {
        headJoint = gameObject.AddComponent<ConfigurableJoint>();
        headJoint.connectedBody = head.GetComponent<Rigidbody>();
        InitJointSetting(headJoint);
        headJoint.anchor = new Vector3(0, -1, 0);
        // Debug.Log("inited head");
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

    private void InitHydraulicSlider() {
        Vector3 temp = transform.GetChild(2).transform.localPosition;
        transform.GetChild(2).transform.localPosition = new Vector3(GetLength() * (hydraulicFactor - 0.5f) / 10, temp.y, temp.z);
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
        // Debug.Log("RRR");
        // Debug.Log(head.IsSplit());
        // Debug.Log(tail.IsSplit());

        // if (head.IsSplit()) {
        //     headSplitNum = 0;
        // }
        // if (tail.IsSplit()) {
        //     tailSplitNum = 0;
        // }
    }

    public void InitTemp(Point point1, Point point2) {
        head = point1;
        tail = point2;
        if (head != null) InitBarHead();
        if (tail != null) InitBarTail();
    }

//    // redundant...........
    public void SetHead(Point point) {
        head = point;
    }

    public void SetTail(Point point) {
        tail = point;
    }

    public void InitHydraulicParams(float factor) {
        hydraulicFactor = factor;
        InitHydraulicSlider();
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
        return GetCurrentTension() / maxLoad; 
    }

    public void SetBaseColor(Color color) {
        baseColor = color;
    }
     
    public Color GetBaseColor() {
        return baseColor;
    }

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

    public void Break() {
        Transform piece1 = transform.GetChild(0);
        Transform piece2 = transform.GetChild(1);
        ActivateBrokenPiece(piece1);
        ActivateBrokenPiece(piece2);
        DisableBar();
    }

    private void ActivateBrokenPiece(Transform piece) {
        piece.gameObject.SetActive(true);
        piece.SetParent(transform.parent, true);
    }

    private void DisableBar() {
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

    public void DeactivateSplit(int i) {
        // 0: head, 1: tail
        if (i == 0) {
            headSplitController.gameObject.SetActive(false);
            headSplitNum = -1;
        } else {
            tailSplitController.gameObject.SetActive(false);
            tailSplitNum = -1;
        }
    }

    public void ActivateSplit(int i) {
        // 0: head, 1: tail
        if (i == 0) {
            // Vector3 newPos = (transform.position + head.transform.position) / 2;
            headSplitController.gameObject.SetActive(true);
            headSplitController.transform.localPosition = new Vector3(-GetLength() / 40, 0, 0);
            if (headSplitNum == -1) headSplitNum = 0;
        } else {
            // Vector3 newPos = (transform.position + tail.transform.position) / 2;
            tailSplitController.gameObject.SetActive(true);
            tailSplitController.transform.localPosition = new Vector3(GetLength() / 40, 0, 0);;
            if (tailSplitNum == -1) tailSplitNum = 0;
        }
    }

    public void ToggleSplitParent(int i) {
        // 0: head, 1: tail
        Color red = new Color(1, 0, 0);
        Color green = new Color(0, 1, 0);
        Color yellow = new Color(1, 197f / 255, 0);
        Color color;
        // Debug.Log("toggled");
        // Debug.Log(i);
        if (i == 0) {
            if (headSplitNum == 1) {
                color = head.IsFixed() ? red : yellow;
                // headSplitNum = 1;
            } else {
                color = green;
                // headSplitNum = 0;
            }
            headSplitController.GetComponent<SpriteRenderer>().material.color = color;
            headSplitNum = (headSplitNum + 1) % 2;
        } else {
            if (tailSplitNum == 1) {
                color = tail.IsFixed() ? red : yellow;
                // tailSplitNum = 1;
            } else {
                color = green;
                // tailSplitNum = 0;
            }
            tailSplitController.GetComponent<SpriteRenderer>().material.color = color;
            tailSplitNum = (tailSplitNum + 1) % 2;
        }

    }




}
