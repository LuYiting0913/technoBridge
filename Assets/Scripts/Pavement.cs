using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pavement : MonoBehaviour {
    private Vector3 headPosition;
    private Vector3 tailPosition;
    private HingeJoint[] hinges = new HingeJoint[4];
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
                Debug.Log(index);
                hinges[index] = gameObject.AddComponent<HingeJoint>();
                hinges[index].connectedBody = p.GetComponent<Rigidbody>();
                hinges[index].axis = new Vector3(0, 0, 1);
                hinges[index].anchor = anchors[index];
            }
        }
    }

    public float GetCurrentTension() {
        return Math.Max(hinges[0] == null ? 0 : hinges[0].currentForce.x, 
                        hinges[2] == null ? 0 : hinges[2].currentForce.x);
    }

    public float GetCurrentLoad() {
        return GetCurrentTension() / maxLoad; // max load implement later
    }

    public void SetBaseColor(Color color) {
        baseColor = color;
    }

    // amend later
    public Color GetLoadColor() {
        float load = GetCurrentLoad();
        if (load < 0.5) {
            return new Color(baseColor.r, 1 - load, baseColor.b);
        } else if (load < 1) {
            return new Color(load, baseColor.g, baseColor.b);
        } else {
            return new Color(1, 0, 0);
        }
    }

    public void DisablePave() {
        for (int i = 0; i < 4; i ++) hinges[i].connectedBody = null;
        this.GetComponent<BoxCollider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        disabled = true;
    }
}


// head = headPoint;
//         headJoint = gameObject.AddComponent<HingeJoint>();
//         headJoint.connectedBody = head.GetComponent<Rigidbody>();
//         headJoint.anchor = new Vector3(0, -1, 0);
//         headJoint.axis = new Vector3(0, 0, 1); 
//         headJoint.breakForce = MaterialManager.GetIntegrity(material);