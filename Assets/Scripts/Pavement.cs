using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pavement : MonoBehaviour {
    private Vector3 headPosition;
    private Vector3 tailPosition;
    public ConfigurableJoint[] hinges = new ConfigurableJoint[4];
    private Vector3[] anchors = new Vector3[4];
    private static float maxLoad = MaterialManager.GetIntegrity(0);
    public bool disabled = false;
    private Color baseColor;

    public static Pavement Instantiate3DPavement(SolidBarReference bar, int roadWidth, Transform parent) {
        Vector3 headPosition = bar.GetHead3D();
        Vector3 tailPosition = bar.GetTail3D();
        Vector2 dir = bar.GetDirection();
        Vector3 midPoint = (headPosition + tailPosition) / 2 + new Vector3(0, 0, roadWidth / 2);
        float angle = Vector2.SignedAngle(Vector2.up, dir);      
        GameObject scaledTemplate = MaterialManager.GetTemplate3D(bar.GetMaterial());

        scaledTemplate.transform.localScale = new Vector3(75, dir.magnitude, 330);

        Pavement newPave = Instantiate(scaledTemplate, midPoint, 
                                        Quaternion.Euler(new Vector3(0, 0, angle)), parent).
                                        GetComponent<Pavement>();

        newPave.SetPosition(headPosition, tailPosition);
        newPave.InitPavementHinge(bar, roadWidth);

        return newPave;
    }

    public void SetPosition(Vector3 head, Vector3 tail) {
        headPosition = head;
        tailPosition = tail;
        anchors[0] = new Vector3(0, -0.5f, -0.15f);
        anchors[1] = new Vector3(0, -0.5f, 0.15f);
        anchors[2] = new Vector3(0, 0.5f, -0.15f);
        anchors[3] = new Vector3(0, 0.5f, 0.15f);
    }

    public void InitPavementHinge(SolidBarReference bar, int roadWidth) {
        Vector3 headMirror = headPosition + new Vector3(0, 0, roadWidth);
        Vector3 tailMirror = tailPosition + new Vector3(0, 0, roadWidth);
        GameObject brokenHead = transform.GetChild(2).gameObject;
        GameObject brokenTail = transform.GetChild(3).gameObject;

        List<Point> points = new List<Point>();

        Point head = bar.GetHeadSplit(AssetManager.GetPoint(headPosition));
        Point tail = bar.GetTailSplit(AssetManager.GetPoint(tailPosition));
        Point headMir = bar.GetHeadSplit(AssetManager.GetPoint(headMirror));
        Point tailMir = bar.GetTailSplit(AssetManager.GetPoint(tailMirror));
        points.Add(head);
        points.Add(headMir);
        points.Add(tail);
        points.Add(tailMir);

        for (int index = 0; index < 4; index++) {
            Rigidbody rb = points[index].GetComponent<Rigidbody>();
            hinges[index] = gameObject.AddComponent<ConfigurableJoint>();
            hinges[index].connectedBody = rb;
            InitJointSetting(hinges[index]);
            hinges[index].anchor = anchors[index];
            
            // broken child
            ConfigurableJoint joint;
            if  (index <= 1) {
                joint = brokenHead.AddComponent<ConfigurableJoint>();
            } else {
                joint = brokenTail.AddComponent<ConfigurableJoint>();
            }
            joint.connectedBody = rb;
            InitJointSetting(joint);
            joint.anchor = anchors[index];
        }

    }

    private void InitJointSetting(ConfigurableJoint joint) {
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.axis = new Vector3(0, 0, 1); 
    }

    public float GetCurrentTension() {
        int valid = 4;
        float force = 0f;
        foreach (ConfigurableJoint joint in hinges) {
            if (joint.currentForce.magnitude < 1) valid -= 1;
            force += joint.currentForce.magnitude;
        }
        // float force = hinges[1].currentForce.x < 1 ? hinges[2].currentForce.x : (hinges[1].currentForce.x + hinges[2].currentForce.x) / 2;
        // Debug.Log(force / valid);
        return force / valid;
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
        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = GetLoadColor();
    }

    public void DisplayNormal() {
        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = GetBaseColor();
    }

    public void Break() {
        Transform piece1 = transform.GetChild(2);
        Transform piece2 = transform.GetChild(3);
        ActivateBrokenPiece(piece1);
        ActivateBrokenPiece(piece2);
        DisablePave();
    }

    private void ActivateBrokenPiece(Transform piece) {
        piece.gameObject.SetActive(true);
        piece.SetParent(transform.parent, true);
    }

    private void DisablePave() {
        // for (int i = 0; i < 4; i ++) Destroy(hinges[i]);
        gameObject.SetActive(false);
        disabled = true;
    }
}


// head = headPoint;
//         headJoint = gameObject.AddComponent<ConfigurableJoint>();
//         headJoint.connectedBody = head.GetComponent<Rigidbody>();
//         headJoint.anchor = new Vector3(0, -1, 0);
//         headJoint.axis = new Vector3(0, 0, 1); 
//         headJoint.breakForce = MaterialManager.GetIntegrity(material);