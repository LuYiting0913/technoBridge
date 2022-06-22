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

    public void SetPosition(Vector3 head, Vector3 tail) {
        headPosition = head;
        tailPosition = tail;
        anchors[0] = new Vector3(0, -0.5f, -0.15f);
        anchors[1] = new Vector3(0, -0.5f, 0.15f);
        anchors[2] = new Vector3(0, 0.5f, -0.15f);
        anchors[3] = new Vector3(0, 0.5f, 0.15f);
    }

    public void InitPavementHinge(List<Point> allPoints, int roadWidth) {
        Vector3 headMirror = headPosition + new Vector3(0, 0, roadWidth);
        Vector3 tailMirror = tailPosition + new Vector3(0, 0, roadWidth);
        GameObject brokenHead = transform.GetChild(2).gameObject;
        GameObject brokenTail = transform.GetChild(3).gameObject;
    	foreach (Point p in allPoints) {
            int index = -1;
            if (p.Contain(headPosition)) {
                index = 0;
            } else if (p.Contain(headMirror)) {
                index = 1;
            } else if (p.Contain(tailPosition)) {
                index = 2;
            } else if (p.Contain(tailMirror)) {
                index = 3;
            }

            if (index != -1) {
                hinges[index] = gameObject.AddComponent<ConfigurableJoint>();
                hinges[index].connectedBody = p.GetComponent<Rigidbody>();
                InitJointSetting(hinges[index]);
                hinges[index].anchor = anchors[index];
    	        
                // broken child
                ConfigurableJoint joint;
                if  (index <= 1) {
                    joint = brokenHead.AddComponent<ConfigurableJoint>();
                } else {
                    joint = brokenTail.AddComponent<ConfigurableJoint>();
                }
                joint.connectedBody = p.GetComponent<Rigidbody>();
                InitJointSetting(joint);
                joint.anchor = anchors[index];
            }
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
        // return Math.Max(hinges[1] == null ? 0 : hinges[1].currentForce.x, 
        //             hinges[2] == null ? 0 : hinges[2].currentForce.x);
        int valid = 4;
        float force = 0f;
        foreach (ConfigurableJoint joint in hinges) {
            if (joint.currentForce.magnitude < 1) valid -= 1;
            force += joint.currentForce.magnitude;
        }
        // float force = hinges[1].currentForce.x < 1 ? hinges[2].currentForce.x : (hinges[1].currentForce.x + hinges[2].currentForce.x) / 2;
        Debug.Log(force / valid);
        return force / valid;
            // Math.Max(hinges[0] == null ? 0 : hinges[0].currentForce.x, 
            //         hinges[1] == null ? 0 : hinges[1].currentForce.x),
            // Math.Max(hinges[2] == null ? 0 : hinges[2].currentForce.x,
            //         hinges[3] == null ? 0 : hinges[3].currentForce.x));
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

    public void DisablePave() {
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