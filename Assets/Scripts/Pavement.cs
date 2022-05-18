using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pavement : MonoBehaviour {
    private Vector3 headPosition;
    private Vector3 tailPosition;
    private HingeJoint[] hinges = new HingeJoint[4];
    private Vector3[] anchors = new Vector3[4];

    public void SetPosition(Vector3 head, Vector3 tail) {
        headPosition = head;
        tailPosition = tail;
        anchors[0] = new Vector3(0, -0.5f, -0.5f);
        anchors[1] = new Vector3(0, -0.5f, 0.5f);
        anchors[2] = new Vector3(0, 0.5f, -0.5f);
        anchors[3] = new Vector3(0, 0.5f, 0.5f);
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
}


// head = headPoint;
//         headJoint = gameObject.AddComponent<HingeJoint>();
//         headJoint.connectedBody = head.GetComponent<Rigidbody>();
//         headJoint.anchor = new Vector3(0, -1, 0);
//         headJoint.axis = new Vector3(0, 0, 1); 
//         headJoint.breakForce = MaterialManager.GetIntegrity(material);